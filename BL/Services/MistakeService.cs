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

namespace BL.Services
{
    public class MistakeService : IMistakeService
    {
        private readonly ScheduleHighSchoolDb _context;

        public MistakeService(ScheduleHighSchoolDb context)
        {
            _context = context;
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        ///<inheritdoc/>
        public async Task<List<string>> GetMistakesByStudyClassAsync(int studyClassId, int versionId)
        {
            ConcurrentBag<string> result = new ConcurrentBag<string>();

            var version = await _context.Versions.FirstOrDefaultAsync(v => v.Id == versionId);

            if (version == null)
            {
                throw new Exception("Версия расписания не найдена!");
            }

            var studyClass = _context.StudyClasses
                .Include(s => s.ClassShift)
                .First(l => l.Id == studyClassId);

            var studyClassLessons = _context.Lessons
                .Include(l => l.StudyClass)
                .Where(l => l.RowIndex != null)
                .Where(l => l.StudyClassId == studyClassId && l.StudyClass.ClassShiftId == studyClass.ClassShiftId);

            var intersectLessons = await _context.Lessons
                .AsQueryable()
                .Where(l => l.VersionId == versionId)
                .Where(l => l.RowIndex != null)
                .Include(l => l.Teacher)
                .Include(l => l.StudyClass)
                    .ThenInclude(s => s.ClassShift)
                .Include(l => l.StudyClass)
                    .ThenInclude(s => s.EducationForm)
                .Include(l => l.Classroom)
                .Include(l => l.Version)
                .Where(l => l.StudyClass.ClassShiftId == studyClass.ClassShiftId)
                .Where(l => studyClassLessons.Any(l2 => l.TeacherId == l2.TeacherId
                                    || (l.ClassroomId == l2.ClassroomId && l.ClassroomId != null && l2.ClassroomId != null)))
                .ToListAsync();

            //TODO
            //Parallel.ForEach пропускает некоторые итерации из-за этого пропускаются накладки. 
            //Необходимо решить проблему либо изменить код не теряя производительность.
            Parallel.ForEach(intersectLessons, lesson =>
            {
                var rowIndexSecond = lesson.RowIndex >= 0 && lesson.RowIndex % 2 != 0 ? lesson.RowIndex - 1 : lesson.RowIndex + 1;

                var conditionString = new StringBuilder();

                if (!lesson.IsSubClassLesson && !lesson.IsSubWeekLesson)
                {
                    conditionString.Append($"(RowIndex == {lesson.RowIndex})");
                    conditionString.Append($" AND ((ClassroomId == {lesson.ClassroomId ?? -1} || TeacherId == {lesson.TeacherId}))");
                    conditionString.Append($" AND ((FlowId == null AND  {lesson.FlowId == null}) || FlowId != {lesson.FlowId ?? -1})");
                }
                else if (!lesson.IsSubClassLesson && lesson.IsSubWeekLesson)
                {
                    conditionString.Append($"(RowIndex == {lesson.RowIndex} || (IsSubWeekLesson == {false} && RowIndex == {rowIndexSecond}))");
                    conditionString.Append($" AND ((ClassroomId == {lesson.ClassroomId ?? -1} || TeacherId == {lesson.TeacherId}))");
                    conditionString.Append($" AND FlowId != {lesson.FlowId ?? -1}");
                }
                else if (lesson.IsSubClassLesson && !lesson.IsSubWeekLesson)
                {
                    conditionString.Append($"(RowIndex == {lesson.RowIndex}" + (version.UseSubWeek ? $" || RowIndex == {rowIndexSecond})" : ""));
                    conditionString.Append($" AND ((ClassroomId == {lesson.ClassroomId ?? -1} || TeacherId == {lesson.TeacherId}))");
                }
                else
                {
                    conditionString.Append($"(RowIndex == {lesson.RowIndex} || RowIndex == {rowIndexSecond})");
                    conditionString.Append($" AND ((ClassroomId == {lesson.ClassroomId ?? -1} || TeacherId == {lesson.TeacherId}))");
                    conditionString.Append($" AND ((FlowId == null AND  {lesson.FlowId == null}) || FlowId != {lesson.FlowId ?? -1})");
                }

                var mistakeLessonList = intersectLessons
                    .AsQueryable()
                    .Where(conditionString.ToString())
                    .Where(l =>l.Id != lesson.Id)
                    .Where(l =>l.StudyClassId != studyClassId);

                var mistakeTeacherLessonList = mistakeLessonList
                    .Where(l => l.TeacherId == lesson.TeacherId);

                var mistakeClassroomLessonList = mistakeLessonList
                    .Where(l => l.ClassroomId == lesson.ClassroomId && l.ClassroomId != null && lesson.ClassroomId != null);

                foreach (var mistake in mistakeTeacherLessonList)
                {
                    var message = $"{GetDayName(mistake.RowIndex.Value, version)} | {GetLessonNumber(mistake.RowIndex.Value, version)} | Накладка по преподавателю: {GetTeacherFIO(mistake.Teacher)} | {mistake.StudyClass.Name}";

                    result.Add(message);
                }

                foreach (var mistake in mistakeClassroomLessonList)
                {
                    var message = $"{GetDayName(mistake.RowIndex.Value, version)} | {GetLessonNumber(mistake.RowIndex.Value, version)} | Накладка по аудитории: {mistake.Classroom.Name} | {mistake.StudyClass.Name}";

                    result.Add(message);
                }
            });

            return result.Distinct().ToList();
        }

        ///<inheritdoc/>
        public async Task<List<string>> GetStudyClassNamesWithMistakesAsync()
        {
            ConcurrentBag<string> result = new ConcurrentBag<string>();

            var version = await _context.Versions.FirstOrDefaultAsync(v => v.IsActive);

            var lessons = await _context.Lessons
                .Include(l => l.Teacher)
                .Include(l => l.Version)
                .Include(l => l.StudyClass)
                    .ThenInclude(s => s.ClassShift)
                .Include(l => l.StudyClass)
                    .ThenInclude(s => s.EducationForm)
                .Where(l => l.VersionId == version.Id)
                .Where(l => l.RowIndex != null)
                .ToListAsync();

            //TODO
            //Parallel.ForEach пропускает некоторые итерации из-за этого пропускаются накладки. 
            //Необходимо решить проблему либо изменить код не теряя производительность.
            Parallel.ForEach(lessons, lesson =>
            {
                var rowIndexSecond = lesson.RowIndex >= 0 && lesson.RowIndex % 2 != 0 ? lesson.RowIndex - 1 : lesson.RowIndex + 1;

                var conditionString = new StringBuilder();

                if (!lesson.IsSubClassLesson && !lesson.IsSubWeekLesson)
                {
                    conditionString.Append($"(RowIndex == {lesson.RowIndex}" + (version.UseSubWeek ? $" || RowIndex == {rowIndexSecond}" : "") + ")");
                    conditionString.Append($" AND ((FlowId == null AND  {lesson.FlowId == null}) || FlowId != {lesson.FlowId ?? -1})");
                }
                else if (!lesson.IsSubClassLesson && lesson.IsSubWeekLesson)
                {
                    conditionString.Append($"(RowIndex == {lesson.RowIndex} || (IsSubWeekLesson == {false} && RowIndex == {rowIndexSecond}))");
                    conditionString.Append($" AND FlowId != {lesson.FlowId ?? -1}");
                }
                else if (lesson.IsSubClassLesson && !lesson.IsSubWeekLesson)
                {
                    conditionString.Append($"RowIndex == {lesson.RowIndex}" + (version.UseSubWeek ? $" || RowIndex == {rowIndexSecond}" : ""));
                }
                else
                {
                    conditionString.Append($"RowIndex == {lesson.RowIndex}");
                }

                var mistakeStudyClassNameList = lessons
                    .AsQueryable()
                    .Where(l => l.Version.IsActive
                        && l.Id != lesson.Id
                        && ((l.Teacher != null && l.TeacherId == lesson.TeacherId)
                            || (l.StudyClassId == lesson.StudyClassId && l.ColIndex == lesson.ColIndex)
                            || (l.ClassroomId == lesson.ClassroomId && l.ClassroomId != null && lesson.ClassroomId != null))
                        && l.StudyClass.ClassShiftId == lesson.StudyClass.ClassShiftId
                    ).Where(conditionString.ToString())
                    .Select(l => l.StudyClass.Name);

                foreach (var mistake in mistakeStudyClassNameList)
                {
                    result.Add(mistake);
                }
            });

            return result.Distinct().OrderBy(s => s).ToList();
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
            string[] days = new string[] { "Понедельник", "Вторник", "Среда", "Четверг", "Пятница", "Суббота", "Воскресенье" };

            string result;

            double lessonCountByDay = version.UseSubWeek ? version.MaxLesson * 2 : version.MaxLesson;

            int dayNumber = (int)Math.Floor(rowIndex / lessonCountByDay);

            result = days[dayNumber];

            return result;
        }
    }
}
