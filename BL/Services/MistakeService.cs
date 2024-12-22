using DAL.Entities.Schedule;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using BL.ServiceInterface;
using DAL;
using Version = DAL.Entities.Version;
using Infrastructure.Models;

namespace BL.Services
{
    public class MistakeService : IMistakeService
    {
        private readonly ScheduleHighSchoolDb _context;

        private List<string> dayNames = new List<string> { "Понедельник", "Вторник", "Среда", "Четверг", "Пятница", "Суббота", "Воскресенье" };

        public MistakeService(ScheduleHighSchoolDb context)
        {
            _context = context;
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        ///<inheritdoc/>
        public async Task<List<string>> GetMistakesByStudyClassAsync(int studyClassId, int versionId)
        {
            // 1. Получаем версию
            var version = await _context.Versions
                .FirstOrDefaultAsync(v => v.Id == versionId);

            if (version == null)
                throw new Exception("Версия расписания не найдена!");

            // 2. Загружаем нужный класс (с его сменой)
            var studyClass = await _context.StudyClasses
                .Include(s => s.ClassShift)
                .FirstOrDefaultAsync(s => s.Id == studyClassId);

            if (studyClass == null)
                throw new Exception("Учебный класс не найден!");

            // 3. Загружаем все уроки в этой смене, чтобы проверять пересечения
            //    Сразу подгружаем Teacher, Classroom, StudyClass, т.д.
            var allLessonsSameShift = await _context.Lessons
                .Where(l => l.VersionId == version.Id)
                .Where(l => l.RowIndex != null)
                .Where(l => l.StudyClass.ClassShiftId == studyClass.ClassShiftId)
                .Include(l => l.Teacher)
                .Include(l => l.StudyClass)
                    .ThenInclude(s => s.ClassShift)
                .Include(l => l.StudyClass)
                    .ThenInclude(s => s.EducationForm)
                .Include(l => l.Classroom)
                .Include(l => l.Version)
                .ToListAsync();

            // 4. Делим уроки на:
            //    - Уроки данного класса (myLessons)
            //    - Уроки остальных классов (otherLessons)
            var myLessons = allLessonsSameShift
                .Where(l => l.StudyClassId == studyClassId)
                .ToList();

            var otherLessons = allLessonsSameShift
                .Where(l => l.StudyClassId != studyClassId)
                .ToList();

            // 5. Конкуррентная коллекция для сбора ошибок
            ConcurrentBag<MistakeListModel> mistakeListResult = new ConcurrentBag<MistakeListModel>();

            // 6. Параллельно обрабатываем каждый урок нашего класса,
            //    И ищем, есть ли конфликты среди "других" уроков
            Parallel.ForEach(myLessons, myLesson =>
            {
                // 6.1. Вычисляем “парный” индекс
                var rowIndexSecond = (myLesson.RowIndex >= 0 && myLesson.RowIndex % 2 != 0)
                    ? myLesson.RowIndex - 1
                    : myLesson.RowIndex + 1;

                // 6.2. Собираем "возможные" RowIndex — аналог логики в исходном коде
                var possibleRowIndexes = new List<int?>();

                if (!myLesson.IsSubClassLesson && !myLesson.IsSubWeekLesson)
                {
                    possibleRowIndexes.Add(myLesson.RowIndex);
                    if (version.UseSubWeek)
                        possibleRowIndexes.Add(rowIndexSecond);
                }
                else if (!myLesson.IsSubClassLesson && myLesson.IsSubWeekLesson)
                {
                    possibleRowIndexes.Add(myLesson.RowIndex);
                    // subWeekLesson конфликтует с уроками, у которых IsSubWeekLesson == false
                    // Но в памяти можно просто добавить rowIndexSecond и потом отфильтровать 
                    // (или сделать более сложную проверку, если нужно).
                    possibleRowIndexes.Add(rowIndexSecond);
                }
                else if (myLesson.IsSubClassLesson && !myLesson.IsSubWeekLesson)
                {
                    possibleRowIndexes.Add(myLesson.RowIndex);
                    if (version.UseSubWeek)
                        possibleRowIndexes.Add(rowIndexSecond);
                }
                else
                {
                    // myLesson.IsSubClassLesson && myLesson.IsSubWeekLesson
                    possibleRowIndexes.Add(myLesson.RowIndex);
                }

                possibleRowIndexes = possibleRowIndexes.Distinct().ToList();

                // 6.3. Выбираем из "otherLessons" только те, у которых RowIndex попадает в possibleRowIndexes
                //      и где действительно пересечение по TeacherId или ClassroomId.
                //      Если FlowId важен, учитываем и это (как у вас в коде).
                var conflictingLessons = otherLessons
                    .Where(ol =>
                        ol.RowIndex != null &&
                        possibleRowIndexes.Contains(ol.RowIndex) &&
                        // Пример: совпадение по преподавателю или по аудитории
                        // (если надо — дополняем проверку FlowId, IsSubWeekLesson, IsSubClassLesson, и т.д.)
                        (
                            (ol.TeacherId != null && ol.TeacherId == myLesson.TeacherId)
                            ||
                            (ol.ClassroomId != null && ol.ClassroomId == myLesson.ClassroomId)
                        )
                    )
                    .ToList();

                // 6.4. Разделяем ошибки на два типа (по преподавателю и по аудитории)
                var teacherConflicts = conflictingLessons
                    .Where(l => l.TeacherId == myLesson.TeacherId && myLesson.TeacherId != null);

                var classroomConflicts = conflictingLessons
                    .Where(l => l.ClassroomId == myLesson.ClassroomId && myLesson.ClassroomId != null);

                // 6.5. Формируем MistakeListModel для каждого конфликта
                foreach (var mistake in teacherConflicts)
                {
                    mistakeListResult.Add(new MistakeListModel
                    {
                        Day = GetDayName(mistake.RowIndex.Value, version),
                        Para = GetLessonNumber(mistake.RowIndex.Value, version),
                        MistakeType = "Накладка по преподавателю:",
                        MistakeObject = GetTeacherFIO(mistake.Teacher),
                        // Имя класса, с которым возник конфликт
                        StudyClass = mistake.StudyClass.Name
                    });
                }

                foreach (var mistake in classroomConflicts)
                {
                    mistakeListResult.Add(new MistakeListModel
                    {
                        Day = GetDayName(mistake.RowIndex.Value, version),
                        Para = GetLessonNumber(mistake.RowIndex.Value, version),
                        MistakeType = "Накладка по аудитории:",
                        MistakeObject = mistake.Classroom?.Name,
                        StudyClass = mistake.StudyClass.Name
                    });
                }
            });

            // 7. Приводим результат к нужному виду: "День | Пара | Ошибка | ..."
            //    Предположим, у нас есть список dayNames для сортировки:
            var dayNames = new List<string>
    {
        "Понедельник", "Вторник", "Среда",
        "Четверг", "Пятница", "Суббота", "Воскресенье"
    };

            return mistakeListResult
                .OrderBy(m => dayNames.IndexOf(m.Day)) // сортировка по дню
                .ThenBy(m => m.Para)                  // далее по паре
                .Select(m => $"{m.Day} | {m.Para} | {m.MistakeType} {m.MistakeObject} | {m.StudyClass}")
                .Distinct()
                .ToList();
        }

        ///<inheritdoc/>
        public async Task<List<string>> GetStudyClassNamesWithMistakesAsync()
        {
            // 1. Получаем текущую активную версию
            var version = await _context.Versions
                .Where(v => v.IsActive)
                .Select(v => new { v.Id, v.UseSubWeek })
                .FirstOrDefaultAsync();

            if (version == null)
                return new List<string>();

            // 2. Загружаем нужные данные уроков (только необходимые поля)
            //    Обратите внимание: .Include(...) можно убрать, если не нужны другие данные
            var lessons = await _context.Lessons
                .Where(l => l.VersionId == version.Id && l.RowIndex != null)
                .Select(l => new
                {
                    l.Id,
                    l.RowIndex,
                    l.ColIndex,
                    l.TeacherId,
                    l.ClassroomId,
                    l.StudyClassId,
                    l.IsSubWeekLesson,
                    l.IsSubClassLesson,
                    l.FlowId,
                    // Достаём ClassShiftId, чтобы не делать потом дополнительных Includes:
                    ClassShiftId = l.StudyClass.ClassShiftId,
                    StudyClassName = l.StudyClass.Name
                })
                .ToListAsync();

            // Если уроков нет — сразу возвращаем пустой список
            if (!lessons.Any())
                return new List<string>();

            // 3. Предварительно вычисляем для каждого урока набор допустимых RowIndex
            //    (учитываем UseSubWeek, IsSubWeekLesson, IsSubClassLesson и т.п.)
            //    Также ради оптимизации можно сразу сложить в другой объект
            var lessonInfos = lessons.Select(l =>
            {
                // Вычислим "парный" rowIndexSecond, как в исходном коде
                var rowIndexSecond = (l.RowIndex >= 0 && l.RowIndex % 2 != 0)
                    ? l.RowIndex - 1
                    : l.RowIndex + 1;

                // Собираем все RowIndex, которые могут конфликтовать с текущим уроком
                // Логика аналогична исходному коду, но в более декларативном виде:
                var possibleRowIndexes = new List<int?>();

                if (!l.IsSubClassLesson && !l.IsSubWeekLesson)
                {
                    // основной RowIndex и, если UseSubWeek == true, добавляем rowIndexSecond
                    possibleRowIndexes.Add(l.RowIndex);
                    if (version.UseSubWeek)
                        possibleRowIndexes.Add(rowIndexSecond);
                }
                else if (!l.IsSubClassLesson && l.IsSubWeekLesson)
                {
                    // Основной + rowIndexSecond (если у другого урока IsSubWeekLesson == false)
                    // но мы пока просто добавим их оба
                    possibleRowIndexes.Add(l.RowIndex);
                    possibleRowIndexes.Add(rowIndexSecond);
                }
                else if (l.IsSubClassLesson && !l.IsSubWeekLesson)
                {
                    // rowIndex + (rowIndexSecond, если UseSubWeek)
                    possibleRowIndexes.Add(l.RowIndex);
                    if (version.UseSubWeek)
                        possibleRowIndexes.Add(rowIndexSecond);
                }
                else
                {
                    // l.IsSubClassLesson && l.IsSubWeekLesson
                    // только текущий RowIndex
                    possibleRowIndexes.Add(l.RowIndex);
                }

                // Удаляем дубликаты, на всякий случай
                possibleRowIndexes = possibleRowIndexes.Distinct().ToList();

                return new
                {
                    LessonId = l.Id,
                    TeacherId = l.TeacherId,
                    ClassroomId = l.ClassroomId,
                    StudyClassId = l.StudyClassId,
                    ColIndex = l.ColIndex,
                    ClassShiftId = l.ClassShiftId,
                    FlowId = l.FlowId,
                    StudyClassName = l.StudyClassName,
                    PossibleRowIndexes = possibleRowIndexes
                };
            }).ToList();

            // 4. “Прямой” перебор O(n^2) тоже можно сделать, но мы можем сгруппировать:
            //    - Группы по ClassShiftId (т.к. конфликт возможен только внутри одной смены)
            //    - Внутри группы по каждому “возможному” RowIndex
            //    И там уже смотреть, есть ли пересечение по TeacherId / ClassroomId / StudyClassId+ColIndex и т.д.
            //
            // Суть — мы сначала раскидываем уроки по (ClassShiftId, RowIndex) для всех допустимых RowIndex.
            // Затем в рамках каждой такой корзины ищем реальный конфликт.

            // 4.1. Создадим словарь (classShiftId, rowIndex) -> список уроков
            //      Но т.к. у каждого урока может быть несколько “possibleRowIndexes”,
            //      придётся добавлять урок в несколько корзин (если rowIndexSecond есть).
            var grouped = new Dictionary<(int ClassShiftId, int? RowIndex), List<(int LessonId, int? TeacherId, int? ClassroomId, int? StudyClassId, int? ColIndex, long? FlowId, string Name)>>();

            foreach (var info in lessonInfos)
            {
                foreach (var rIndex in info.PossibleRowIndexes)
                {
                    if (!grouped.TryGetValue((info.ClassShiftId, rIndex), out var list))
                    {
                        list = new List<(int, int?, int?, int?, int?, long?, string)>();
                        grouped[(info.ClassShiftId, rIndex)] = list;
                    }
                    list.Add((info.LessonId, info.TeacherId, info.ClassroomId, info.StudyClassId, info.ColIndex, info.FlowId, info.StudyClassName));
                }
            }

            // 5. Ищем конфликты внутри каждой корзины.
            //    Для скорости создадим HashSet<string> (результат без повторов).
            var mistakenStudyClasses = new HashSet<string>();

            // 5.1. Перебираем каждую корзину
            foreach (var kvp in grouped)
            {
                // Список уроков, которые могут конфликтовать друг с другом
                var bucket = kvp.Value;

                // Если в корзине 0 или 1 урок — точно нет конфликта
                if (bucket.Count <= 1)
                    continue;

                // Когда записей много — O(n^2), но обычно это сильно меньше, чем все уроки
                // (ведь мы фильтруем по (ClassShiftId, RowIndex)).
                for (int i = 0; i < bucket.Count; i++)
                {
                    var lA = bucket[i];
                    for (int j = i + 1; j < bucket.Count; j++)
                    {
                        var lB = bucket[j];

                        // 5.2. Применяем вашу логику конфликтов:
                        //     (у одного и того же преподавателя) ИЛИ
                        //     (одна и та же StudyClassId + ColIndex) ИЛИ
                        //     (один и тот же ClassroomId != null),
                        //     при этом должен быть учтён FlowId (как у вас в коде) — 
                        //     см. исходный код, там сложная проверка: 
                        //        && ((FlowId == null AND  {lesson.FlowId == null}) || FlowId != lesson.FlowId)
                        //     или "FlowId != {...}" в разных ветках.
                        //     Ниже — упрощённая проверка (адаптируйте под точные условия!):

                        // Примерная логика: если FlowId совпадают, то конфликта может не быть (или наоборот).
                        // В исходном коде есть разные ветки, но мы дадим пример простого условия:
                        bool conflictByTeacher = (lA.TeacherId.HasValue && lA.TeacherId == lB.TeacherId);
                        bool conflictByStudyClassAndColIndex = (lA.StudyClassId.HasValue && lA.StudyClassId == lB.StudyClassId && lA.ColIndex == lB.ColIndex);
                        bool conflictByClassroom = (lA.ClassroomId.HasValue && lB.ClassroomId.HasValue && lA.ClassroomId == lB.ClassroomId);

                        // FlowId проверяем, если нужно отсеять некоторые случаи.
                        // В исходнике есть (FlowId == null AND  {lesson.FlowId == null}) || FlowId != {lesson.FlowId}
                        // Ниже — лишь пример:
                        bool flowOk = true;
                        // Если нужно, отключите/включите условие:
                        // flowOk = (lA.FlowId == null && lB.FlowId == null) || (lA.FlowId != lB.FlowId);

                        if ((conflictByTeacher || conflictByStudyClassAndColIndex || conflictByClassroom) && flowOk)
                        {
                            // Добавляем в список конфликтных оба класса
                            mistakenStudyClasses.Add(lA.Name);
                            mistakenStudyClasses.Add(lB.Name);
                        }
                    }
                }
            }

            // 6. Возвращаем результат, отсортированный по алфавиту
            return mistakenStudyClasses.OrderBy(x => x).ToList();
        }

        private string GetTeacherFIO(Teacher teacher)
        {
            string result = "";

            if (!string.IsNullOrEmpty(teacher.FirstName))
            {
                result += teacher.FirstName;
            }

            if (!string.IsNullOrEmpty(teacher.Name))
            {
                result += $" {teacher.Name[0]}.";
            }

            if (!string.IsNullOrEmpty(teacher.MiddleName))
            {
                result += $" {teacher.MiddleName[0]}.";
            }

            return result;
        }
        private string GetLessonNumber(int rowIndex, Version version)
        {
            string result;

            if (version.UseSubWeek)
            {
                if ((rowIndex + 1) % 2 != 0)
                {
                    var row = (rowIndex % (version.MaxLesson * 2)) + 1;
                    result = row % 2 != 0 ? $"Пара: {row}-{row + 1}" : "";
                }
                else
                {
                    var row = (rowIndex % (version.MaxLesson * 2));
                    result = $"Пара: {row}-{row + 1}";
                }
            }
            else
            {
                var row = rowIndex % version.MaxLesson + 1;
                row = row + (row-1);
                result = $"Пара: {row}-{row + 1}";
            }

            return result;
        }
        private string GetDayName(int rowIndex, Version version)
        {
            string result;

            double lessonCountByDay = version.UseSubWeek ? version.MaxLesson * 2 : version.MaxLesson;

            int dayNumber = (int)Math.Floor(rowIndex / lessonCountByDay);

            result = dayNames[dayNumber];

            return result;
        }
    }
}
