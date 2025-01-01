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
using DocumentFormat.OpenXml.Bibliography;

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
            var version = await _context.Versions.FirstOrDefaultAsync(v => v.Id == versionId)
                ?? throw new Exception("Версия расписания не найдена!");

            var studyClass = await _context.StudyClasses
                .Include(s => s.ClassShift)
                .FirstOrDefaultAsync(l => l.Id == studyClassId)
                ?? throw new Exception("Учебная группа не найдена!");

            // Получаем все занятия текущей группы
            var currentClassLessons = await _context.Lessons
                .Where(l => l.VersionId == versionId
                    && l.RowIndex != null
                    && l.StudyClassId == studyClassId)
                .Include(l => l.Teacher)
                .Include(l => l.Flow)
                .Include(l => l.StudyClass)
                .Include(l => l.Classroom)
                .ToListAsync();

            // Получаем все потенциально пересекающиеся занятия в той же смене
            var allIntersectingLessons = await _context.Lessons
                .Where(l => l.VersionId == versionId
                    && l.RowIndex != null
                    && l.StudyClass.ClassShiftId == studyClass.ClassShiftId)
                .Include(l => l.Teacher)
                .Include(l => l.Flow)
                .Include(l => l.StudyClass)
                .Include(l => l.Classroom)
                .ToListAsync();

            var conflicts = new ConcurrentBag<MistakeListModel>();

            foreach (var lesson in currentClassLessons)
            {
                var rowIndexSecond = GetSecondWeekRowIndex(lesson.RowIndex.Value);
                var timeSlotLessons = GetLessonsInTimeSlot(allIntersectingLessons, lesson, rowIndexSecond, version.UseSubWeek);

                foreach (var otherLesson in timeSlotLessons.Where(l => l.Id != lesson.Id))
                {
                    // Пропускаем проверку, если оба занятия в одном потоке
                    if (lesson.FlowId != null && otherLesson.FlowId != null && lesson.FlowId == otherLesson.FlowId)
                        continue;

                    // Получаем список ID преподавателей из каждого занятия
                    var lesson1TeacherIds = new HashSet<int>();
                    var lesson2TeacherIds = new HashSet<int>();

                    // Добавляем основного преподавателя
                    lesson1TeacherIds.Add(lesson.TeacherId);
                    lesson2TeacherIds.Add(otherLesson.TeacherId);

                    // Добавляем преподавателей из потока
                    if (lesson.Flow?.TeacherList != null)
                        foreach (var teacher in lesson.Flow.TeacherList)
                            lesson1TeacherIds.Add(teacher.Id);

                    if (otherLesson.Flow?.TeacherList != null)
                        foreach (var teacher in otherLesson.Flow.TeacherList)
                            lesson2TeacherIds.Add(teacher.Id);

                    // Находим пересекающихся преподавателей
                    var intersectingTeacherId = lesson1TeacherIds.Intersect(lesson2TeacherIds).FirstOrDefault();
                    if (intersectingTeacherId != 0)
                    {
                        conflicts.Add(CreateTeacherConflict(otherLesson, version, intersectingTeacherId));
                    }

                    // Проверяем конфликт по аудитории
                    if (HasClassroomConflict(lesson, otherLesson))
                    {
                        conflicts.Add(CreateClassroomConflict(otherLesson, version));
                    }
                }
            }

            return FormatConflicts(conflicts);
        }

        private bool HasClassroomConflict(Lesson lesson1, Lesson lesson2)
        {
            return lesson1.ClassroomId != null &&
                   lesson2.ClassroomId != null &&
                   lesson1.ClassroomId == lesson2.ClassroomId;
        }

        private IEnumerable<Lesson> GetLessonsInTimeSlot(List<Lesson> allLessons, Lesson currentLesson, int secondWeekIndex, bool useSubWeek)
        {
            return allLessons.Where(l =>
            {
                if (currentLesson.IsSubWeekLesson || l.IsSubWeekLesson)
                {
                    // Для занятий по одной неделе проверяем только конкретный индекс
                    return l.RowIndex == currentLesson.RowIndex;
                }
                else
                {
                    // Для занятий на обе недели проверяем оба индекса если используются поднедели
                    return l.RowIndex == currentLesson.RowIndex ||
                           (useSubWeek && l.RowIndex == secondWeekIndex);
                }
            });
        }

        private int GetSecondWeekRowIndex(int rowIndex) =>
            rowIndex >= 0 && rowIndex % 2 != 0 ? rowIndex - 1 : rowIndex + 1;

        private MistakeListModel CreateTeacherConflict(Lesson lesson, Version version, int conflictingTeacherId) =>
            new MistakeListModel
            {
                Day = GetDayName(lesson.RowIndex.Value, version),
                Para = GetLessonNumber(lesson.RowIndex.Value, version),
                MistakeType = "Накладка по преподавателю: ",
                MistakeObject = GetTeacherFIO(conflictingTeacherId == lesson.TeacherId
                    ? lesson.Teacher
                    : lesson.Flow?.TeacherList?.FirstOrDefault(t => t.Id == conflictingTeacherId)),
                StudyClass = lesson.StudyClass.Name
            };

        private MistakeListModel CreateClassroomConflict(Lesson lesson, Version version) =>
            new MistakeListModel
            {
                Day = GetDayName(lesson.RowIndex.Value, version),
                Para = GetLessonNumber(lesson.RowIndex.Value, version),
                MistakeType = "Накладка по аудитории: ",
                MistakeObject = lesson.Classroom.Name,
                StudyClass = lesson.StudyClass.Name
            };

        private List<string> FormatConflicts(ConcurrentBag<MistakeListModel> conflicts)
        {
            return conflicts
                .GroupBy(m => new { m.Day, m.Para, m.MistakeType, m.MistakeObject })
                .Select(group => new
                {
                    group.Key.Day,
                    group.Key.Para,
                    group.Key.MistakeType,
                    group.Key.MistakeObject,
                    StudyClasses = string.Join(", ", group.Select(m => m.StudyClass).Distinct())
                })
                .OrderBy(g => dayNames.IndexOf(g.Day))
                .ThenBy(g => g.Para)
                .Select(g => $"{g.Day} | {g.Para} | {g.MistakeType} {g.MistakeObject} | {g.StudyClasses}")
                .ToList();
        }

        ///<inheritdoc/>
        public async Task<List<string>> GetStudyClassNamesWithMistakesAsync()
                {
                    // 1. Получаем активную версию
                    var version = await _context.Versions
                        .Where(v => v.IsActive)
                        .Select(v => new { v.Id, v.UseSubWeek })
                        .FirstOrDefaultAsync();

                    if (version == null)
                        return new List<string>();

                    // 2. Загружаем уроки с информацией о потоках
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
                            Flow = l.Flow, // Теперь включаем информацию о потоке
                            ClassShiftId = l.StudyClass.ClassShiftId,
                            StudyClassName = l.StudyClass.Name
                        })
                        .ToListAsync();

                    if (!lessons.Any())
                        return new List<string>();

                    // 3. Создаем вспомогательный класс для хранения информации об уроке
                    var lessonInfos = lessons.Select(l =>
                    {
                        var rowIndexSecond = (l.RowIndex >= 0 && l.RowIndex % 2 != 0)
                            ? l.RowIndex - 1
                            : l.RowIndex + 1;

                        var possibleRowIndexes = new List<int?>();

                        if (!l.IsSubClassLesson && !l.IsSubWeekLesson)
                        {
                            possibleRowIndexes.Add(l.RowIndex);
                            if (version.UseSubWeek)
                                possibleRowIndexes.Add(rowIndexSecond);
                        }
                        else if (!l.IsSubClassLesson && l.IsSubWeekLesson)
                        {
                            possibleRowIndexes.Add(l.RowIndex);
                            possibleRowIndexes.Add(rowIndexSecond);
                        }
                        else if (l.IsSubClassLesson && !l.IsSubWeekLesson)
                        {
                            possibleRowIndexes.Add(l.RowIndex);
                            if (version.UseSubWeek)
                                possibleRowIndexes.Add(rowIndexSecond);
                        }
                        else
                        {
                            possibleRowIndexes.Add(l.RowIndex);
                        }

                        return new
                        {
                            LessonId = l.Id,
                            TeacherId = l.TeacherId,
                            ClassroomId = l.ClassroomId,
                            StudyClassId = l.StudyClassId,
                            ColIndex = l.ColIndex,
                            ClassShiftId = l.ClassShiftId,
                            FlowId = l.FlowId,
                            Flow = l.Flow,
                            StudyClassName = l.StudyClassName,
                            PossibleRowIndexes = possibleRowIndexes.Distinct().ToList()
                        };
                    }).ToList();

                    // 4. Группировка по сменам и строкам
                    var grouped = new Dictionary<(int ClassShiftId, int? RowIndex), List<(
                        int LessonId,
                        int? TeacherId,
                        int? ClassroomId,
                        int? StudyClassId,
                        int? ColIndex,
                        long? FlowId,
                        Flow Flow,
                        string Name)>>();

                    foreach (var info in lessonInfos)
                    {
                        foreach (var rIndex in info.PossibleRowIndexes)
                        {
                            if (!grouped.TryGetValue((info.ClassShiftId, rIndex), out var list))
                            {
                                list = new List<(int, int?, int?, int?, int?, long?, Flow, string)>();
                                grouped[(info.ClassShiftId, rIndex)] = list;
                            }
                            list.Add((info.LessonId, info.TeacherId, info.ClassroomId, info.StudyClassId,
                                     info.ColIndex, info.FlowId, info.Flow, info.StudyClassName));
                        }
                    }

                    var mistakenStudyClasses = new HashSet<string>();

                    // 5. Проверка конфликтов с учетом потоков
                    foreach (var kvp in grouped)
                    {
                        var bucket = kvp.Value;
                        if (bucket.Count <= 1) continue;

                        for (int i = 0; i < bucket.Count; i++)
                        {
                            var lA = bucket[i];
                            for (int j = i + 1; j < bucket.Count; j++)
                            {
                                var lB = bucket[j];

                                // Если уроки в одном потоке - пропускаем проверку
                                if (lA.FlowId.HasValue && lB.FlowId.HasValue && lA.FlowId == lB.FlowId)
                                    continue;

                                bool hasConflict = false;

                                // Проверка конфликта по преподавателю
                                if (lA.TeacherId.HasValue && lB.TeacherId.HasValue)
                                {
                                    // Проверяем основного преподавателя
                                    if (lA.TeacherId == lB.TeacherId)
                                    {
                                        hasConflict = true;
                                    }
                                    // Проверяем преподавателей из потока
                                    else if (lA.Flow?.TeacherList != null && lB.Flow?.TeacherList != null)
                                    {
                                        var teachersA = lA.Flow.TeacherList;
                                        var teachersB = lB.Flow.TeacherList;

                                        if (teachersA.Any(t => t.Id == lB.TeacherId) ||
                                            teachersB.Any(t => t.Id == lA.TeacherId) ||
                                            teachersA.Any(ta => teachersB.Any(tb => ta.Id == tb.Id)))
                                        {
                                            hasConflict = true;
                                        }
                                    }
                                }

                                // Проверка конфликта по учебному классу и ColIndex
                                if (!hasConflict && lA.StudyClassId.HasValue && lB.StudyClassId.HasValue &&
                                    lA.ColIndex == lB.ColIndex)
                                {
                                    // Проверяем основной класс
                                    if (lA.StudyClassId == lB.StudyClassId)
                                    {
                                        hasConflict = true;
                                    }
                                    // Проверяем классы из потока
                                    else if (lA.Flow?.StudyClassList != null && lB.Flow?.StudyClassList != null)
                                    {
                                        var classesA = lA.Flow.StudyClassList;
                                        var classesB = lB.Flow.StudyClassList;

                                        if (classesA.Any(c => c.Id == lB.StudyClassId) ||
                                            classesB.Any(c => c.Id == lA.StudyClassId) ||
                                            classesA.Any(ca => classesB.Any(cb => ca.Id == cb.Id)))
                                        {
                                            hasConflict = true;
                                        }
                                    }
                                }

                                // Проверка конфликта по кабинету
                                if (!hasConflict && lA.ClassroomId.HasValue && lB.ClassroomId.HasValue &&
                                    lA.ClassroomId == lB.ClassroomId)
                                {
                                    hasConflict = true;
                                }

                                if (hasConflict)
                                {
                                    mistakenStudyClasses.Add(lA.Name);
                                    mistakenStudyClasses.Add(lB.Name);
                                }
                            }
                        }
                    }

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
                row = row + (row - 1);
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
