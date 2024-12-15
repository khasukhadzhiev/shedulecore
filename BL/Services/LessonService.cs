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
                flowLesson.Flow.StudyClassList = flow.StudyClassList.Select(t => t.ToStudyClassDto()).ToList();
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
            lessonDto.Flow.StudyClassList = lessonDto.Flow.StudyClassList.Distinct().ToList();

            if (lessonDto.Flow.StudyClassList.Count > 0)
            {
                    try
                    {
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

                        var flowName = string.Join(", ", lessonDto.Flow.StudyClassList.Select(s=>s.Name));


                        var flow = new Flow { 
                            Name = flowName,
                            TeacherList = lessonDto.Flow.TeacherList.Select(t=>t.ToTeacher()).ToList(),
                            StudyClassList = lessonDto.Flow.StudyClassList.Select(t => t.ToStudyClass()).ToList(),
                        };

                        await _context.Flows.AddAsync(flow);
                        await _context.SaveChangesAsync();

                        lessonDto.FlowId = flow.Id;
                        lessonDto.VersionId = versionId;

                        //Добавление поточного занятия
                        foreach (var studyClass in lessonDto.Flow.StudyClassList)
                        {
                            lessonDto.StudyClassId = studyClass.Id;
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
            var version = await _context.Versions
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.Id == versionId);

            if (version == null)
            {
                throw new ArgumentException("Version not found");
            }

            // Компиляция условий LINQ
            IQueryable<Lesson> query = _context.Lessons
                .AsNoTracking()
                .Include(l => l.LessonType)
                .Include(l => l.StudyClass)
                .Include(l => l.Subject)
                .Include(l => l.Teacher)
                .Include(l => l.Flow)
                .Include(l => l.Version)
                .Where(l => l.VersionId == versionId && l.StudyClassId == studyClassId);

            if (!version.UseSubWeek && !version.UseSubClass)
            {
                query = query.Where(l => !l.IsSubClassLesson && !l.IsSubWeekLesson);
            }
            else if (version.UseSubWeek && !version.UseSubClass)
            {
                query = query.Where(l =>
                    (l.IsSubWeekLesson && !l.IsSubClassLesson) || (!l.IsSubClassLesson && !l.IsSubWeekLesson));
            }
            else if (!version.UseSubWeek && version.UseSubClass)
            {
                query = query.Where(l =>
                    (!l.IsSubWeekLesson && l.IsSubClassLesson) || (!l.IsSubClassLesson && !l.IsSubWeekLesson));
            }

            var lessonList = await query
                .OrderBy(l => l.Subject.Name)
                .ThenBy(l => l.LessonType.Id)
                .Select(l => l.ToLessonDto())
                .ToListAsync();

            // Предварительная загрузка данных о потоках
            var flowIds = lessonList
                .Where(l => l.FlowId != null)
                .Select(l => l.FlowId.Value)
                .Distinct()
                .ToList();

            var flows = await _context.Flows
                .AsNoTracking()
                .Where(f => flowIds.Contains(f.Id))
                .ToDictionaryAsync(f => f.Id, f => f.TeacherList.Select(t => t.ToTeacherDto()).ToList());

            // Заполнение данных о потоках
            foreach (var lesson in lessonList)
            {
                if (lesson.FlowId != null && flows.TryGetValue(lesson.FlowId.Value, out var teacherList))
                {
                    lesson.Flow.TeacherList = teacherList;
                }
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

            lessonDto.Flow.StudyClassList = lessonDto.Flow.StudyClassList.DistinctBy(s=>s.Id).ToList();

            if (lesson.FlowId != null)
            {
                var flowName = string.Join(", ", lessonDto.Flow.StudyClassList.Select(s => s.Name));

                var flowLessonList = await _context.Lessons
                    .Include(l=>l.StudyClass)
                    .Include(l=>l.Flow).Where(l => l.FlowId == lesson.FlowId).ToListAsync();

                var studyClassToRemove = flowLessonList.Select(l=>l.StudyClass.Id).Except(lessonDto.Flow.StudyClassList.Select(l => l.Id));

                var studyClassOld = flowLessonList.Select(l => l.StudyClass.Id).Intersect(lessonDto.Flow.StudyClassList.Select(l => l.Id));

                var studyClassNewList = lessonDto.Flow.StudyClassList.Select(l => l.Id).Except(flowLessonList.Select(l => l.StudyClass.Id));

                foreach (var studyClassNew in studyClassNewList)
                {
                    var newLesson = new Lesson
                    {
                        LessonTypeId = lessonDto.LessonType.Id,

                        TeacherId = lesson.TeacherId,

                        FlowId = lessonDto.FlowId,

                        SubjectId = lessonDto.Subject.Id,

                        StudyClassId = studyClassNew,

                        VersionId = versionId,

                        IsSubClassLesson = lessonDto.IsSubClassLesson,

                        IsSubWeekLesson = lessonDto.IsSubWeekLesson,

                        RowIndex = lesson.RowIndex,

                        ColIndex = lesson.ColIndex,
                    };

                    await _context.Lessons.AddAsync(newLesson);
                }

                foreach (var flowLesson in flowLessonList)
                {
                    var flow = await _context.Flows.FirstOrDefaultAsync(f => f.Id == flowLesson.FlowId.Value);
                    
                    flow.Name = flowName;

                    if (studyClassToRemove.Contains(flowLesson.StudyClassId))
                    {
                        _context.Lessons.Remove(flowLesson);

                        continue;
                    }

                    flowLesson.Flow.TeacherList = lessonDto.Flow.TeacherList.Select(t => t.ToTeacher()).ToList();

                    flowLesson.Flow.StudyClassList = lessonDto.Flow.StudyClassList.Select(t => t.ToStudyClass()).ToList();

                    flowLesson.LessonTypeId = lessonDto.LessonType.Id;

                    flowLesson.TeacherId = flow.TeacherList.ToList().First().Id;

                    flowLesson.Flow.Name = flowName;

                    flowLesson.SubjectId = lessonDto.Subject.Id;

                    flowLesson.IsSubClassLesson = lessonDto.IsSubClassLesson;

                    flowLesson.IsSubWeekLesson = lessonDto.IsSubWeekLesson;
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
