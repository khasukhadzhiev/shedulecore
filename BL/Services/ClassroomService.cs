using BL.ServiceInterface;
using DTL.Dto.ScheduleDto;
using DTL.Mapping;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL;
using DTL.Dto;

namespace BL.Services
{
    public class ClassroomService : IClassroomService
    {
        private readonly ScheduleHighSchoolDb _context;

        public ClassroomService(ScheduleHighSchoolDb context)
        {
            _context = context;
        }

        ///<inheritdoc/>
        public async Task<List<ClassroomTypeDto>> GetClassroomTypeListAsync()
        {
            var classroomTypeList = await _context.ClassroomTypes
                .AsNoTracking()
                .Select(s => s.ToClassroomTypeDto())
                .ToListAsync();

            return classroomTypeList;
        }

        ///<inheritdoc/>
        public async Task<List<ClassroomDto>> GetClassroomListAsync()
        {
            var classroomList = await _context.Classrooms
                .AsNoTracking()
                .Include(c => c.Building)
                .Select(s => s.ToClassroomDto())
                .ToListAsync();

            return classroomList;
        }

        ///<inheritdoc/>
        public async Task<string> AddClassroomAsync(ClassroomDto classroomDto)
        {
            if (string.IsNullOrEmpty(classroomDto.Name.Trim()) || classroomDto.SeatsCount < 1)
            {
                throw new Exception("Наименование и количетство посадочных мест должно быть заполнено.");
            }

            string result = null;

            var classroomNew = classroomDto.ToClassroom();

            if (classroomNew.Name.Contains(","))
            {
                var buildingClassroomNameList = await _context.Classrooms.Where(c => c.BuildingId == classroomNew.BuildingId).Select(c => c.Name).ToListAsync();

                var newClassroomNames = classroomNew.Name.Split(",").Except(buildingClassroomNameList).Where(n => n.Length > 0).ToList();

                foreach (var name in newClassroomNames)
                {
                    classroomDto.Name = name;

                    await AddClassroomAsync(classroomDto);
                }

                await _context.SaveChangesAsync();
            }
            else
            {
                var exist = await _context.Classrooms.AnyAsync(c => c.Name == classroomNew.Name && c.BuildingId == classroomNew.BuildingId);

                if (exist)
                {
                    result = "Такая аудитория уже есть!";
                }
                else
                {
                    await _context.Classrooms.AddAsync(classroomNew);

                    await _context.SaveChangesAsync();
                }
            }

            return result;
        }

        ///<inheritdoc/>
        public async Task<string> EditClassroomAsync(ClassroomDto classroomDto)
        {
            if (string.IsNullOrEmpty(classroomDto.Name.Trim()) || classroomDto.SeatsCount < 1)
            {
                throw new Exception("Наименование и количетство посадочных мест должно быть заполнено.");
            }

            string result = null;

            var classroomNew = classroomDto.ToClassroom();

            var exist = await _context.Classrooms.AnyAsync(c => c.Name == classroomNew.Name && c.Id != classroomNew.Id && c.BuildingId == classroomNew.BuildingId);

            if (exist)
            {
                result = "Такая аудитория уже есть!";
            }
            else
            {
                var classroomOld = await _context.Classrooms.FirstOrDefaultAsync(s => s.Id == classroomDto.Id);

                classroomOld.Name = classroomNew.Name.Contains(",") ? classroomNew.Name.Split(",").First() : classroomNew.Name;
                classroomOld.BuildingId = classroomNew.BuildingId;

                await _context.SaveChangesAsync();
            }

            return result;
        }

        ///<inheritdoc/>
        public async Task RemoveClassroomAsync(int id)
        {
            var classroom = await _context.Classrooms.FirstOrDefaultAsync(s => s.Id == id);

            if (classroom != null)
            {
                _context.Classrooms.Remove(classroom);

                await _context.SaveChangesAsync();
            }
        }

        ///<inheritdoc/>
        public async Task SetClassroomByLessonAsync(int lessonId, int classroomId)
        {            
            var lesson = await _context.Lessons
                .FirstOrDefaultAsync(l => l.Id == lessonId);       

            if (lesson.FlowId != null)
            {
                var flows = _context.Lessons.Where(l => l.FlowId == lesson.FlowId);

                foreach (var flow in flows)
                {
                    flow.ClassroomId = classroomId;
                }
            }

            lesson.ClassroomId = classroomId;

            await _context.SaveChangesAsync();

        }


        ///<inheritdoc/>
        public async Task<ClassroomSetResponseDto> GetWarningsByClassroomAsync(int lessonId)
        {
            var lesson = await _context.Lessons
                .Include(l => l.StudyClass)
                .FirstOrDefaultAsync(l => l.Id == lessonId);

            var teacher = await _context.Teachers
                .FirstOrDefaultAsync(t => t.Id == lesson.TeacherId);

            var classroom = await _context.Classrooms
                .Include(c => c.Building)
                .FirstOrDefaultAsync(c => c.Id == lesson.ClassroomId);

            if(classroom == null)
            {
                return null;
            }

            var teacherLessonList = await _context.Lessons
                .Include(l => l.Classroom)
                .Include(l => l.Version)
                .Where(l => l.ClassroomId != null)
                .Where(l => l.Id != lesson.Id)
                .Where(l => l.TeacherId == teacher.Id)
                .Where(l => l.RowIndex != null)
                .ToListAsync();

            var teacherWarningToChangeBuilding = teacherLessonList
                .Where(l => l.LessonDay != null && l.LessonDay == lesson.LessonDay)
                .Where(l => l.Classroom.BuildingId != classroom.BuildingId)
                .Any(l => Math.Abs(l.LessonNumber.Value - lesson.LessonNumber.Value) < 2);

            var classroomSetResponseDto = new ClassroomSetResponseDto();

            //Проверяем посадочные места
            if (lesson.StudyClass.StudentsCount > classroom.SeatsCount)
            {
                classroomSetResponseDto.ShowMessage = true;
                classroomSetResponseDto.MessageList.Add("Количество посадочных мест в аудитории, меньше чем количество студентов группы!");
            }

            if (teacherWarningToChangeBuilding)
            {
                classroomSetResponseDto.ShowMessage = true;
                classroomSetResponseDto.MessageList.Add("Занятия данного преподавателя идут подряд в разных корпусах. Убедитесь, что преподаватель имеет достаточное время для переезда!");
            }

            return classroomSetResponseDto;
        }

        ///<inheritdoc/>
        public async Task<List<QueryScheduleDto>> GetScheduleByClassroomsAsync(List<ClassroomDto> selectedClassroomListDto, int versionId)
        {
            if (selectedClassroomListDto.Count == 0)
            {
                return null;
            }

            List<QueryScheduleDto> scheduleList = new List<QueryScheduleDto>();

            foreach (var classroom in selectedClassroomListDto)
            {
                var lessons = await _context.Lessons
                    .Include(l => l.LessonType)
                    .Include(l => l.Classroom)
                        .ThenInclude(c => c.Building)
                    .Include(l => l.Flow)
                    .Include(l => l.StudyClass)
                    .Include(l => l.Subject)
                    .Include(l => l.Teacher)
                    .Where(l => l.ClassroomId == classroom.Id)
                    .Where(l => l.VersionId == versionId)
                    .OrderBy(l => l.LessonTypeId)
                    .AsNoTracking()
                    .ToListAsync();


                QueryScheduleDto schedule;

                schedule = new QueryScheduleDto
                {
                    Name = classroom.BuildingDto.Name + " " + classroom.Name,
                    IsStudyClass = true,
                    LessonList = lessons.Select(l => l.ToLessonDto()).ToList()
                };

                var exist = scheduleList.FirstOrDefault(t => t.Name == schedule.Name);

                if (exist == null)
                {
                    scheduleList.Add(schedule);
                }
            }

            var result = scheduleList.Count > 0 ? scheduleList : null;

            return result;
        }
    }
}
