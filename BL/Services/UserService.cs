using DTL.Dto.ScheduleDto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using BL.ServiceInterface;
using DAL;
using DTL.Mapping;
using ScheduleVersion = DAL.Entities.Version;


namespace BL.Services
{
    public class UserService : IUserService
    {
        private readonly ScheduleHighSchoolDb _context;

        private readonly ScheduleVersion _version;
        public UserService(ScheduleHighSchoolDb context)
        {
            _context = context;
            _version = _context.Versions.FirstOrDefault(v => v.IsActive);
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public async Task<List<string>> GetQueryOptionListAsync()
        {
            var studyClassList = await _context.StudyClasses
                .Select(s => s.Name)
                .OrderBy(o => o)
                                .ToListAsync();

            var teacherList = await _context.Teachers
                .Select(t => t.FirstName + " " + t.Name + " " + t.MiddleName)
                .OrderBy(o => o)
                .ToListAsync();

            return studyClassList.Concat(teacherList).ToList();
        }

        ///<inheritdoc/>
        public async Task<List<QueryScheduleDto>> GetScheduleAsync(string queryString)
        {
            if (string.IsNullOrEmpty(queryString) || string.IsNullOrWhiteSpace(queryString))
            {
                return null;
            }

            var queryList = queryString.Split(",");

            List<QueryScheduleDto> scheduleList = new List<QueryScheduleDto>();

            foreach (var query in queryList)
            {
                var studyClass = await _context.StudyClasses
                    .Include(s => s.EducationForm)
                    .Include(s => s.ClassShift)
                    .Include(s => s.Subdivision)
                    .FirstOrDefaultAsync(t => t.Name.Replace(" ", "")
                    .Contains(query.Replace(" ", "").Trim().ToUpper()));

                var teacher = await _context.Teachers
                    .FirstOrDefaultAsync(t => (t.FirstName + " " + t.Name + " " + t.MiddleName)
                    .Contains(query.Trim().ToUpper()));

                if (studyClass == null && teacher == null)
                {
                    continue;
                }

                var conditionString = new StringBuilder();

                string paramName = studyClass != null ? "StudyClassId" : teacher != null ? "TeacherId" : null;

                int paramValue = studyClass != null ? studyClass.Id : teacher != null ? teacher.Id : -1;

                conditionString.Append($"{paramName} == {paramValue} AND ");

                if (_version.UseSubWeek && _version.UseSubClass)
                {
                    conditionString.Append($"{paramName} == {paramValue} AND RowIndex != null");
                }
                else if (_version.UseSubWeek == false && _version.UseSubClass == false)
                {
                    conditionString.Append($"IsSubClassLesson == {false} AND IsSubWeekLesson == {false}");
                }
                else if (_version.UseSubWeek && _version.UseSubClass == false)
                {
                    conditionString.Append($"((IsSubWeekLesson == {true} AND IsSubClassLesson == {false}) OR (IsSubClassLesson == {false} AND IsSubWeekLesson == {false}))");
                }
                else if (_version.UseSubWeek == false && _version.UseSubClass)
                {
                    conditionString.Append($"((IsSubWeekLesson == {false} AND IsSubClassLesson == {true})  OR (IsSubClassLesson == {false} AND IsSubWeekLesson == {false}))");
                }
                else
                {
                    conditionString.Append($"((IsSubClassLesson == {true}) OR (IsSubClassLesson == {false} AND IsSubWeekLesson == {false}))");
                }

                var lessons = await _context.Lessons
                    .Include(l => l.LessonType)
                    .Include(l => l.Classroom)
                        .ThenInclude(l => l.Building)
                    .Include(l => l.Flow)
                    .Include(l => l.StudyClass)
                    .Include(l => l.Subject)
                    .Include(l => l.Teacher)
                    .Where(conditionString.ToString())
                    .Where(l => l.VersionId == _version.Id)
                    .OrderBy(l => l.LessonTypeId)
                    .ToListAsync();


                QueryScheduleDto schedule;

                if (paramName == "StudyClassId")
                {
                    schedule = new QueryScheduleDto
                    {
                        Id = studyClass.Id,
                        Name = studyClass.Name,
                        ClassShiftId = studyClass.ClassShiftId,
                        ClassShift = studyClass.ClassShift.ToClassShiftDto(),
                        EducationFormId = studyClass.EducationFormId,
                        EducationForm = studyClass.EducationForm.ToEducationFormDto(),
                        StudentsCount = studyClass.StudentsCount,
                        SubdivisionId = studyClass.SubdivisionId,
                        Subdivision = studyClass.Subdivision.ToSubdivisionDto(),
                        IsStudyClass = true,
                        LessonList = lessons.Select(l => l.ToLessonDto()).ToList()
                    };
                }
                else
                {
                    schedule = new QueryScheduleDto
                    {
                        Name = $"{teacher.FirstName} {teacher.Name} {teacher.MiddleName}",
                        IsStudyClass = false,
                        LessonList = lessons.Select(l => l.ToLessonDto()).ToList()
                    };
                }

                var exist = scheduleList.FirstOrDefault(t => t.Name == schedule.Name);

                if (exist == null)
                {
                    scheduleList.Add(schedule);
                }
            }

            var result = scheduleList.Count > 0 ? scheduleList : null;

            return result;
        }

        public async Task<List<QueryScheduleReportingDto>> GetReportingScheduleAsync(string queryString)
        {
            if (string.IsNullOrEmpty(queryString) || string.IsNullOrWhiteSpace(queryString))
            {
                return null;
            }

            var queryList = queryString.Split(",");

            var showReportingIdList = _version.ShowReportingIds.Select(i => Convert.ToInt32(i)).ToList();

            List<QueryScheduleReportingDto> scheduleReportingList = new List<QueryScheduleReportingDto>();

            foreach (var query in queryList)
            {
                var studyClass = await _context.StudyClasses
                    .FirstOrDefaultAsync(r => r.Name.Replace(" ", "")
                    .Contains(query.Replace(" ", "").Trim().ToUpper()));

                var teacher = await _context.Teachers
                    .FirstOrDefaultAsync(t => (t.FirstName + " " + t.Name + " " + t.MiddleName)
                    .Contains(query.Trim().ToUpper()));

                if (studyClass == null && teacher == null)
                {
                    continue;
                }

                var conditionString = new StringBuilder();

                string paramName = studyClass != null ? "StudyClassId" : teacher != null ? "TeacherId" : null;

                int paramValue = studyClass != null ? studyClass.Id : teacher != null ? teacher.Id : -1;

                conditionString.Append($"{paramName} == {paramValue}");

                var scheduleReporting = await _context.StudyClassReporting
                    .Include(r => r.StudyClass)
                    .Include(r => r.Subject)
                    .Include(r => r.Teacher)
                    .Include(r => r.ReportingType)
                    .Include(r => r.Classroom)
                        .ThenInclude(c => c.Building)
                    .Where(r => r.VersionId == _version.Id)
                    .Where(conditionString.ToString())
                    .Where(r => showReportingIdList.Contains(r.ReportingTypeId))
                    .Select(r => new QueryScheduleReportingDto
                    {
                        StudyClassId = r.StudyClassId,
                        StudyClassName = r.StudyClass.Name,
                        SubjectId = r.SubjectId,
                        SubjectName = r.Subject.Name,
                        TeacherId = r.TeacherId,
                        TeacherName = r.Teacher.FirstName + " " + r.Teacher.Name + " " + r.Teacher.MiddleName,
                        ReportingTypeId = r.ReportingTypeId,
                        ReportingTypeName = r.ReportingType.Name,
                        ClassroomId = r.ClassroomId,
                        ClassroomName = r.ClassroomId != null ? r.Classroom.Building.Name + " " + r.Classroom.Name : null,
                        Date = r.Date,
                        VersionId = r.VersionId,
                    })
                    .ToListAsync();



                scheduleReportingList.AddRange(scheduleReporting);
            }

            var result = scheduleReportingList.Count > 0 ? scheduleReportingList : null;

            return result;
        }
    }
}
