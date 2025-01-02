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
using DTL.Dto.ScheduleDto;
using DAL.Entities;

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
            // Получаем версию и учебную группу
            var version = await _context.Versions.FirstOrDefaultAsync(v => v.Id == versionId)
                ?? throw new Exception("Версия расписания не найдена!");

            var studyClass = await _context.StudyClasses
                .Include(s => s.ClassShift)
                .FirstOrDefaultAsync(l => l.Id == studyClassId)
                ?? throw new Exception("Учебная группа не найдена!");

            // Получаем все занятия текущей группы и потенциально пересекающиеся занятия
            var currentClassLessons = await _context.Lessons
                .Where(l => l.VersionId == versionId &&
                            l.RowIndex != null &&
                            l.StudyClassId == studyClassId)
                .Include(l => l.Teacher)
                .Include(l => l.Flow)
                .Include(l => l.StudyClass)
                .Include(l => l.Classroom)
                .ToListAsync();

            var allIntersectingLessons = await _context.Lessons
                .Where(l => l.VersionId == versionId &&
                            l.RowIndex != null &&
                            l.StudyClass.ClassShiftId == studyClass.ClassShiftId)
                .Include(l => l.Teacher)
                .Include(l => l.Flow)
                .Include(l => l.StudyClass)
                .Include(l => l.Classroom)
                .ToListAsync();

            var conflicts = new ConcurrentBag<MistakeListModel>();

            // Проверяем конфликты для каждого занятия
            foreach (var lesson in currentClassLessons)
            {
                // Получаем все занятия в том же временном слоте
                var timeSlotLessons = allIntersectingLessons.Where(l =>
                {
                    if (l.Id == lesson.Id) return false;

                    var currentIndex = lesson.RowIndex!.Value;
                    var otherIndex = l.RowIndex!.Value;
                    var secondWeekIndex = otherIndex >= 0 && otherIndex % 2 != 0 ? otherIndex - 1 : otherIndex + 1;

                    // Проверяем совпадение по времени с учетом подгрупп
                    var baseWeekMatches = currentIndex == otherIndex;
                    var oppositeWeekMatches = version.UseSubWeek && currentIndex == secondWeekIndex;

                    if (!lesson.IsSubWeekLesson || !l.IsSubWeekLesson)
                    {
                        return baseWeekMatches || oppositeWeekMatches;
                    }

                    return baseWeekMatches;
                });

                foreach (var otherLesson in timeSlotLessons)
                {
                    // Пропускаем проверку для занятий в одном потоке
                    if (lesson.FlowId != null && otherLesson.FlowId != null &&
                        lesson.FlowId == otherLesson.FlowId)
                        continue;

                    // Проверяем конфликты преподавателей
                    var lesson1Teachers = new HashSet<int> { lesson.TeacherId };
                    var lesson2Teachers = new HashSet<int> { otherLesson.TeacherId };

                    if (lesson.Flow?.TeacherList != null)
                        lesson1Teachers.UnionWith(lesson.Flow.TeacherList.Select(t => t.Id));
                    if (otherLesson.Flow?.TeacherList != null)
                        lesson2Teachers.UnionWith(otherLesson.Flow.TeacherList.Select(t => t.Id));

                    var intersectingTeacherId = lesson1Teachers.Intersect(lesson2Teachers).FirstOrDefault();
                    if (intersectingTeacherId != 0)
                    {
                        conflicts.Add(new MistakeListModel
                        {
                            Day = GetDayName(otherLesson.RowIndex!.Value, version),
                            Para = GetLessonNumber(otherLesson.RowIndex!.Value, version),
                            MistakeType = "Накладка по преподавателю: ",
                            MistakeObject = GetTeacherFIO(intersectingTeacherId == otherLesson.TeacherId
                                ? otherLesson.Teacher
                                : otherLesson.Flow?.TeacherList?.FirstOrDefault(t => t.Id == intersectingTeacherId)),
                            StudyClass = otherLesson.StudyClass.Name
                        });
                    }

                    // Проверяем конфликты аудиторий
                    if (lesson.ClassroomId != null && otherLesson.ClassroomId != null &&
                        lesson.ClassroomId == otherLesson.ClassroomId)
                    {
                        conflicts.Add(new MistakeListModel
                        {
                            Day = GetDayName(otherLesson.RowIndex!.Value, version),
                            Para = GetLessonNumber(otherLesson.RowIndex!.Value, version),
                            MistakeType = "Накладка по аудитории: ",
                            MistakeObject = otherLesson.Classroom!.Name,
                            StudyClass = otherLesson.StudyClass.Name
                        });
                    }
                }
            }

            // Форматируем и возвращаем результат
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
            // Получаем версию расписания
            var version = await _context.Versions.FirstAsync(v => v.IsActive);

            // Получаем все занятия с их связями
            var allLessons = await _context.Lessons
                .Where(l => l.VersionId == version.Id && l.RowIndex != null)
                .Include(l => l.Teacher)
                .Include(l => l.Flow)
                .Include(l => l.StudyClass)
                    .ThenInclude(s => s.ClassShift)
                .Include(l => l.Classroom)
                .ToListAsync();

            var studyClassesWithConflicts = new HashSet<string>();

            // Группируем занятия по сменам для оптимизации проверки
            var lessonsByShift = allLessons.GroupBy(l => l.StudyClass.ClassShiftId);

            foreach (var shiftGroup in lessonsByShift)
            {
                var shiftLessons = shiftGroup.ToList();

                // Проверяем каждое занятие в смене
                foreach (var lesson in shiftLessons)
                {
                    // Получаем все занятия в том же временном слоте
                    var timeSlotLessons = shiftLessons.Where(l =>
                    {
                        if (l.Id == lesson.Id) return false;

                        var currentIndex = lesson.RowIndex!.Value;
                        var otherIndex = l.RowIndex!.Value;
                        var secondWeekIndex = otherIndex >= 0 && otherIndex % 2 != 0 ? otherIndex - 1 : otherIndex + 1;

                        // Проверяем совпадение по времени с учетом подгрупп
                        var baseWeekMatches = currentIndex == otherIndex;
                        var oppositeWeekMatches = version.UseSubWeek && currentIndex == secondWeekIndex;

                        if (!lesson.IsSubWeekLesson || !l.IsSubWeekLesson)
                        {
                            return baseWeekMatches || oppositeWeekMatches;
                        }

                        return baseWeekMatches;
                    });

                    foreach (var otherLesson in timeSlotLessons)
                    {
                        // Пропускаем проверку для занятий в одном потоке
                        if (lesson.FlowId != null && otherLesson.FlowId != null &&
                            lesson.FlowId == otherLesson.FlowId)
                            continue;

                        bool hasConflict = false;

                        // Проверяем конфликты преподавателей
                        var lesson1Teachers = new HashSet<int> { lesson.TeacherId };
                        var lesson2Teachers = new HashSet<int> { otherLesson.TeacherId };

                        if (lesson.Flow?.TeacherList != null)
                            lesson1Teachers.UnionWith(lesson.Flow.TeacherList.Select(t => t.Id));
                        if (otherLesson.Flow?.TeacherList != null)
                            lesson2Teachers.UnionWith(otherLesson.Flow.TeacherList.Select(t => t.Id));

                        if (lesson1Teachers.Intersect(lesson2Teachers).Any())
                        {
                            hasConflict = true;
                        }

                        // Проверяем конфликты аудиторий
                        if (lesson.ClassroomId != null && otherLesson.ClassroomId != null &&
                            lesson.ClassroomId == otherLesson.ClassroomId)
                        {
                            hasConflict = true;
                        }

                        if (hasConflict)
                        {
                            studyClassesWithConflicts.Add(lesson.StudyClass.Name);
                            studyClassesWithConflicts.Add(otherLesson.StudyClass.Name);
                        }
                    }
                }
            }

            // Возвращаем отсортированный список групп с накладками
            return studyClassesWithConflicts.OrderBy(name => name).ToList();
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
