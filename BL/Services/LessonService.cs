using DAL.Entities.Schedule;
using DTL.Dto.ScheduleDto;
using DTL.Mapping;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using BL.ServiceInterface;
using DAL;
using System.Text.Json;

namespace BL.Services
{
    public class LessonService : ILessonService
    {
        private readonly ScheduleHighSchoolDb _context;

        public LessonService(ScheduleHighSchoolDb context)
        {
            _context = context;
        }

        ///<inheritdoc/>
        public async Task<List<LessonDto>> GetMainLessonListAsync(int studyClassId, int versionId)
        {
            var lessonList = await _context.Lessons
                .AsNoTracking()
                .Include(l => l.LessonType)
                .Include(l => l.StudyClass)
                .Include(l => l.Subject)
                .Include(l => l.Teacher)
                .Include(l => l.Version)
                .Where(l => l.StudyClassId == studyClassId
                            && l.FlowId == null
                            && !l.IsParallel
                            && l.VersionId == versionId)
                .OrderBy(l => l.Subject.Name).ThenBy(l => l.LessonType.Id)
                .Select(l => l.ToLessonDto())
                .ToListAsync();

            return lessonList;
        }

        ///<inheritdoc/>
        public async Task<List<LessonDto>> GetFlowLessonListAsync(int studyClassId, int versionId)
        {
            var flowLessonList = await _context.Lessons
                .AsNoTracking()
                .Include(l => l.LessonType)
                .Include(l => l.StudyClass)
                .Include(l => l.Subject)
                .Include(l => l.Teacher)
                .Include(l => l.Version)
                .Include(l => l.Flow)
                .Where(f => f.StudyClassId == studyClassId
                            && f.FlowId != null
                            && f.VersionId == versionId)
                .OrderBy(l => l.Subject.Name).ThenBy(l => l.LessonType.Id)
                .Select(f => f.ToLessonDto())
                .ToListAsync();

            foreach (var flowLesson in flowLessonList)
            {
                var flow = await _context.Flows
                    .AsNoTracking()
                    .FirstOrDefaultAsync(f => f.Id == flowLesson.FlowId.Value);

                flowLesson.Flow.TeacherList = flow.TeacherList.Select(t => t.ToTeacherDto()).ToList();
                flowLesson.Flow.Name = flow.Name;
                flowLesson.FlowStudyClassNames = (await _context.Flows.FirstOrDefaultAsync(f => f.Id == flowLesson.FlowId.Value)).Name;
            }

            return flowLessonList;
        }

        ///<inheritdoc/>
        public async Task<List<LessonDto>> GetParallelLessonListAsync(int studyClassId, int versionId)
        {
            var lessonList = await _context.Lessons
                .AsNoTracking()
                .Include(p => p.LessonType)
                .Include(p => p.StudyClass)
                .Include(p => p.Subject)
                .Include(p => p.Teacher)
                .Include(l => l.Version)
                .Where(p => p.StudyClassId == studyClassId
                            && p.IsParallel
                            && p.VersionId == versionId)
                .OrderBy(l => l.Subject.Name).ThenBy(l => l.LessonType.Id)
                .Select(l => l.ToLessonDto())
                .ToListAsync();

            return lessonList;
        }

        ///<inheritdoc/>
        public async Task AddMainLessonAsync(LessonDto lessonDto, int versionId)
        {
            var lesson = lessonDto.ToLesson();

            lesson.VersionId = versionId;

            if (lesson.Teacher != null)
            {
                lesson.TeacherId = lesson.Teacher.Id;

                lesson.Teacher = null;
            }

            if (lesson.Subject != null)
            {
                lesson.SubjectId = lesson.Subject.Id;

                lesson.Subject = null;
            }

            await _context.Lessons.AddAsync(lesson);

            await _context.SaveChangesAsync();
        }

        ///<inheritdoc/>
        public async Task AddFlowLessonAsync(LessonDto lessonDto, int versionId)
        {
            if (lessonDto.FlowStudyClassIds.Length > 0)
            {
                    try
                    {
                        lessonDto.FlowStudyClassIds = lessonDto.FlowStudyClassIds.Distinct().ToArray();

                        if (lessonDto.Flow.TeacherList.Any())
                        {
                            lessonDto.TeacherId = lessonDto.Flow.TeacherList.First().Id;

                            lessonDto.Teacher = null;
                        }

                        if (lessonDto.Subject != null)
                        {
                            lessonDto.SubjectId = lessonDto.Subject.Id;

                            lessonDto.Subject = null;
                        }

                        var flowName = string.Join(", ",
                            (await _context.StudyClasses.Where(s => lessonDto.FlowStudyClassIds.Contains(s.Id))
                                .Select(s => s.Name).OrderBy(s => s).ToListAsync())
                            );


                        var flow = new Flow { 
                            Name = flowName,
                            TeacherList = lessonDto.Flow.TeacherList.Select(t=>t.ToTeacher()).ToList(),
                        };

                        await _context.Flows.AddAsync(flow);
                        await _context.SaveChangesAsync();

                        lessonDto.FlowId = flow.Id;
                        lessonDto.VersionId = versionId;

                        //Добавление поточного занятия
                        foreach (var studyClassId in lessonDto.FlowStudyClassIds)
                        {
                            lessonDto.StudyClassId = studyClassId;
                            await _context.Lessons.AddAsync(lessonDto.ToLesson());
                        }
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {

                        throw new Exception(ex.Message);
                    }
            }
        }

        ///<inheritdoc/>
        public async Task AddParallelLessonAsync(LessonDto lessonDto, int versionId)
        {
            var lesson = lessonDto.ToLesson();

            lesson.VersionId = versionId;

            if (lesson.Teacher != null)
            {
                lesson.TeacherId = lesson.Teacher.Id;

                lesson.Teacher = null;
            }

            if (lesson.Subject != null)
            {
                lesson.SubjectId = lesson.Subject.Id;

                lesson.Subject = null;
            }

            await _context.Lessons.AddAsync(lesson);

            await _context.SaveChangesAsync();
        }

        ///<inheritdoc/>
        public async Task RemoveLessonAsync(int lessonId)
        {
            var lesson = await _context.Lessons.FirstOrDefaultAsync(l => l.Id == lessonId);

            if (lesson != null)
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        if (lesson.FlowId != null)
                        {
                            var flows = _context.Lessons.Where(l => l.FlowId == lesson.FlowId);

                            _context.Lessons.RemoveRange(flows);

                            var flow = await _context.Flows.FirstOrDefaultAsync(f => f.Id == lesson.FlowId);

                            _context.Flows.Remove(flow);
                        }
                        else
                        {
                            _context.Lessons.Remove(lesson);
                        }

                        await _context.SaveChangesAsync();

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();

                        throw new Exception(ex.Message);
                    }
                }
            }
        }

        ///<inheritdoc/>
        public async Task<List<LessonTypeDto>> GetLessonTypeListAsync()
        {
            var lessonTypeList = await _context.LessonTypes
                .AsNoTracking()
                .Select(lt => lt.ToDto<LessonTypeDto>())
                .ToListAsync();

            return lessonTypeList;
        }

        ///<inheritdoc/>
        public async Task<List<LessonDto>> GetLessonListAsync(int studyClassId, int versionId)
        {
            var version = await _context.Versions.FirstOrDefaultAsync(v => v.Id == versionId);

            var conditionString = new StringBuilder();

            if (version.UseSubWeek && version.UseSubClass)
            {
                conditionString.Append($"StudyClassId == {studyClassId}");
            }
            else if (version.UseSubWeek == false && version.UseSubClass == false)
            {
                conditionString.Append($"StudyClassId == {studyClassId}");
                conditionString.Append($"AND IsSubClassLesson == {false} AND IsSubWeekLesson == {false}");
            }
            else if (version.UseSubWeek && version.UseSubClass == false)
            {
                conditionString.Append($"StudyClassId == {studyClassId}");
                conditionString.Append($"AND ((IsSubWeekLesson == {true} AND IsSubClassLesson == {false}) || (IsSubClassLesson == {false} AND IsSubWeekLesson == {false}))");
            }
            else if (version.UseSubWeek == false && version.UseSubClass)
            {
                conditionString.Append($"StudyClassId == {studyClassId}");
                conditionString.Append($"AND ((IsSubWeekLesson == {false} AND IsSubClassLesson == {true}) || (IsSubClassLesson == {false} AND IsSubWeekLesson == {false}))");
            }
            else
            {
                conditionString.Append($"StudyClassId == {studyClassId}");
                conditionString.Append($"AND (IsSubClassLesson == {true} || (IsSubClassLesson == {false} AND IsSubWeekLesson == {false}))");
            }

            var lessonList = await _context.Lessons
                .AsNoTracking()
                .Include(l => l.LessonType)
                .Include(l => l.StudyClass)
                .Include(l => l.Subject)
                .Include(l => l.Teacher)
                .Include(l => l.Flow)
                .Include(l => l.Version)
                .Where(conditionString.ToString())
                .Where(l => l.VersionId == versionId)
                .OrderBy(l => l.Subject.Name).ThenBy(l => l.LessonType.Id)
                .Select(l => l.ToLessonDto())
                .ToListAsync();

            foreach (var lesson in lessonList)
            {
                var flow = await _context.Flows
                 .AsNoTracking()
                 .FirstOrDefaultAsync(f => f.Id == lesson.FlowId.Value);

                lesson.Flow.TeacherList = flow.TeacherList.Select(t => t.ToTeacherDto()).ToList();
            }

            return lessonList;

        }

        ///<inheritdoc/>
        public async Task LessonSetAsync(LessonDto lessonDto)
        {
            var lesson = await _context.Lessons.FirstOrDefaultAsync(l => l.Id == lessonDto.Id);

            lesson.RowIndex = lessonDto.RowIndex;

            lesson.ColIndex = lessonDto.ColIndex;

            lesson.ClassroomId = lessonDto.ClassroomId;

            if (lesson.FlowId != null)
            {
                var flows = _context.Lessons.Where(l => l.FlowId == lesson.FlowId);

                foreach (var flow in flows)
                {
                    flow.RowIndex = lessonDto.RowIndex;

                    flow.ColIndex = lessonDto.ColIndex;

                    flow.ClassroomId = lessonDto.ClassroomId;
                }
            }

            await _context.SaveChangesAsync();
        }

        ///<inheritdoc/>
        public async Task ResetAllLessonsAsync(int studyClassId)
        {
            var lessons = await _context.Lessons.Where(l => l.StudyClassId == studyClassId && l.RowIndex != null).ToListAsync();

            foreach (var lesson in lessons)
            {
                if (lesson.FlowId != null)
                {
                    var flows = _context.Lessons.Where(l => l.FlowId == lesson.FlowId);

                    foreach (var flow in flows)
                    {
                        flow.RowIndex = null;

                        flow.ColIndex = null;

                        flow.ClassroomId = null;
                    }
                }

                lesson.RowIndex = null;
                lesson.ColIndex = null;
                lesson.ClassroomId = null;
            }

            await _context.SaveChangesAsync();
        }

        ///<inheritdoc/>
        public async Task EditLessonDataAsync(LessonDto lessonDto, int versionId)
        {
            var lesson = await _context.Lessons.Include(l =>l.Subject).Include(l=> l.Teacher).FirstOrDefaultAsync(l => l.Id == lessonDto.Id);

            if (lesson.FlowId != null)
            {
                var flows = _context.Lessons.Include(l => l.Subject).Include(l => l.Teacher).Where(l => l.FlowId == lesson.FlowId);

                foreach (var flow in flows)
                {
                    flow.LessonTypeId = lessonDto.LessonType.Id;

                    flow.TeacherId = lessonDto.Teacher.Id;

                    flow.SubjectId = lessonDto.Subject.Id;

                    flow.IsSubClassLesson = lessonDto.IsSubClassLesson;

                    flow.IsSubWeekLesson = lessonDto.IsSubWeekLesson;

                    flow.Flow.TeacherList = lessonDto.Flow.TeacherList.Select(t=>t.ToTeacher()).ToList();
                }
            }
            else
            {
                lesson.LessonTypeId = lessonDto.LessonType.Id;

                lesson.TeacherId = lessonDto.Teacher.Id;

                lesson.SubjectId = lessonDto.Subject.Id;

                lesson.IsSubClassLesson = lessonDto.IsSubClassLesson;

                lesson.IsSubWeekLesson = lessonDto.IsSubWeekLesson;
            }

            await _context.SaveChangesAsync();
        }

        ///<inheritdoc/>
        public async Task<List<LessonDto>> GetMainLessonFilterListAsync(int studyClassId, int versionId, string filter)
        {
            var lessonList = await _context.Lessons
                .AsNoTracking()
                .Include(l => l.LessonType)
                .Include(l => l.StudyClass)
                .Include(l => l.Subject)
                .Include(l => l.Teacher)
                .Include(l => l.Version)
                .Where(l => l.StudyClassId == studyClassId
                            && l.FlowId == null
                            && !l.IsParallel
                            && l.VersionId == versionId)
                .Where(l => l.Subject.Name.Contains(filter) || (l.Teacher.FirstName + l.Teacher.Name + l.Teacher.MiddleName).Contains(filter))
                .OrderBy(l => l.Subject.Name).ThenBy(l => l.LessonType.Id)
                .Select(l => l.ToLessonDto())
                .ToListAsync();

            return lessonList;
        }

        ///<inheritdoc/>
        public async Task<List<LessonDto>> GetFlowLessonFilterListAsync(int studyClassId, int versionId, string filter)
        {
            var flowLessonList = await _context.Lessons
                .AsNoTracking()
                .Include(l => l.LessonType)
                .Include(l=> l.Flow)
                    .ThenInclude(f=>f.TeacherList)
                .Include(l => l.Subject)
                .Include(l => l.StudyClass)
                .Include(l => l.Subject)
                .Include(l => l.Teacher)
                .Include(l => l.Version)
                .Where(f => f.StudyClassId == studyClassId
                            && f.FlowId != null
                            && f.VersionId == versionId)
                .Where(l => l.Subject.Name.Contains(filter) || (l.Teacher.FirstName + l.Teacher.Name + l.Teacher.MiddleName).Contains(filter))
                .OrderBy(l => l.Subject.Name).ThenBy(l => l.LessonType.Id)
                .Select(f => f.ToLessonDto())
                .ToListAsync();

            foreach (var flowLesson in flowLessonList)
            {
                flowLesson.FlowStudyClassNames = (await _context.Flows.FirstOrDefaultAsync(f => f.Id == flowLesson.FlowId.Value)).Name;
            }

            return flowLessonList;
        }

        ///<inheritdoc/>
        public async Task<List<LessonDto>> GetParallelLessonFilterListAsync(int studyClassId, int versionId, string filter)
        {
            var lessonList = await _context.Lessons
                .AsNoTracking()
                .Include(p => p.LessonType)
                .Include(p => p.StudyClass)
                .Include(p => p.Subject)
                .Include(p => p.Teacher)
                .Include(l => l.Version)
                .Where(p => p.StudyClassId == studyClassId
                            && p.IsParallel
                            && p.VersionId == versionId)
                .Where(l => l.Subject.Name.Contains(filter) || (l.Teacher.FirstName + l.Teacher.Name + l.Teacher.MiddleName).Contains(filter))
                .OrderBy(l => l.Subject.Name).ThenBy(l => l.LessonType.Id)
                .Select(l => l.ToLessonDto())
                .ToListAsync();

            return lessonList;
        }
    }
}
