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
            // Загружаем версию и класс
            var version = await _context.Versions.AsNoTracking().FirstOrDefaultAsync(v => v.Id == versionId);
            if (version == null) throw new Exception("Версия расписания не найдена!");

            var studyClass = await _context.StudyClasses
                .AsNoTracking()
                .Include(s => s.ClassShift)
                .FirstOrDefaultAsync(s => s.Id == studyClassId);
            if (studyClass == null) throw new Exception("Учебный класс не найден!");

            // Загружаем уроки
            var allLessons = await _context.Lessons
                .AsNoTracking()
                .Where(l => l.VersionId == version.Id && l.RowIndex != null && l.StudyClass.ClassShiftId == studyClass.ClassShiftId)
                .Include(l => l.Teacher)
                .Include(l => l.StudyClass).ThenInclude(s => s.ClassShift)
                .Include(l => l.StudyClass).ThenInclude(s => s.EducationForm)
                .Include(l => l.Classroom)
                .Include(l => l.Flow)
                .ToListAsync();

            var myLessons = allLessons.Where(l => l.StudyClassId == studyClassId).ToList();
            var otherLessons = allLessons.Where(l => l.StudyClassId != studyClassId).ToList();

            var mistakeListResult = new ConcurrentBag<MistakeListModel>();

            // Обработка уроков
            foreach (var myLesson in myLessons)
            {
                var possibleRowIndexes = new HashSet<int?> { myLesson.RowIndex };
                if (version.UseSubWeek)
                {
                    var alternateRowIndex = (myLesson.RowIndex >= 0 && myLesson.RowIndex % 2 != 0) ? myLesson.RowIndex - 1 : myLesson.RowIndex + 1;
                    possibleRowIndexes.Add(alternateRowIndex);
                }

                var conflictingLessons = otherLessons
                    .Where(ol => ol.RowIndex != null && possibleRowIndexes.Contains(ol.RowIndex) && (myLesson.Flow?.Id != ol.Flow?.Id))
                    .ToList();

                foreach (var conflict in conflictingLessons)
                {
                    if (IsTeacherConflict(myLesson, conflict))
                    {
                        AddMistake(mistakeListResult, "Накладка по преподавателю:", GetTeacherFIO(conflict.Teacher), conflict, version);
                    }

                    if (IsClassroomConflict(myLesson, conflict))
                    {
                        AddMistake(mistakeListResult, "Накладка по аудитории:", conflict.Classroom?.Name, conflict, version);
                    }

                    if (IsStudyClassConflict(myLesson, conflict))
                    {
                        AddMistake(mistakeListResult, "Накладка по группе:", conflict.StudyClass.Name, conflict, version);
                    }
                }
            }

            // Форматирование результата
            var dayNames = new[] { "Понедельник", "Вторник", "Среда", "Четверг", "Пятница", "Суббота", "Воскресенье" };
            return mistakeListResult
                .OrderBy(m => Array.IndexOf(dayNames, m.Day))
                .ThenBy(m => m.Para)
                .Select(m => $"{m.Day} | {m.Para} | {m.MistakeType} {m.MistakeObject} | {m.StudyClass}")
                .Distinct()
                .ToList();
        }


        private bool IsTeacherConflict(Lesson myLesson, Lesson otherLesson)
        {
            if (myLesson.TeacherId == otherLesson.TeacherId)
                return true;

            var myTeachers = myLesson.Flow?.TeacherList?.Select(t => t.Id) ?? Enumerable.Empty<int>();
            var otherTeachers = otherLesson.Flow?.TeacherList?.Select(t => t.Id) ?? Enumerable.Empty<int>();

            return myTeachers.Any(t => t == otherLesson.TeacherId) ||
                   otherTeachers.Any(t => t == myLesson.TeacherId) ||
                   myTeachers.Intersect(otherTeachers).Any();
        }

        private bool IsClassroomConflict(Lesson myLesson, Lesson otherLesson)
        {
            return myLesson.ClassroomId == otherLesson.ClassroomId;
        }

        private bool IsStudyClassConflict(Lesson myLesson, Lesson otherLesson)
        {
            if (myLesson.StudyClassId == otherLesson.StudyClassId)
                return true;

            var myClasses = myLesson.Flow?.StudyClassList?.Select(c => c.Id) ?? Enumerable.Empty<int>();
            var otherClasses = otherLesson.Flow?.StudyClassList?.Select(c => c.Id) ?? Enumerable.Empty<int>();

            return myClasses.Any(c => c == otherLesson.StudyClassId) ||
                   otherClasses.Any(c => c == myLesson.StudyClassId) ||
                   myClasses.Intersect(otherClasses).Any();
        }

        private void AddMistake(ConcurrentBag<MistakeListModel> mistakeList, string mistakeType, string mistakeObject, Lesson lesson, Version version)
        {
            mistakeList.Add(new MistakeListModel
            {
                Day = GetDayName(lesson.RowIndex.Value, version),
                Para = GetLessonNumber(lesson.RowIndex.Value, version),
                MistakeType = mistakeType,
                MistakeObject = mistakeObject,
                StudyClass = lesson.StudyClass.Name
            });
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
