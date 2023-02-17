using BL.ServiceInterface;
using DTL.Dto.ScheduleDto;
using DTL.Mapping;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL;

namespace BL.Services
{
    public class TeacherService : ITeacherService
    {
        private readonly ScheduleHighSchoolDb _context;

        public TeacherService(ScheduleHighSchoolDb context)
        {
            _context = context;
        }

        ///<inheritdoc/>
        public async Task<List<TeacherDto>> GetTeacherListAsync()
        {
            var teacherList = await _context.Teachers
                .AsNoTracking()
                .OrderBy(t => t.FirstName)
                .ThenBy(t => t.Name)
                .ThenBy(t => t.MiddleName)
                .Select(s => s.ToTeacherDto())
                .ToListAsync();

            return teacherList;
        }

        ///<inheritdoc/>
        public async Task<string> AddTeacherAsync(TeacherDto teacherDto, int? versionId = null)
        {
            if (string.IsNullOrEmpty(teacherDto.FirstName.Trim()) || string.IsNullOrEmpty(teacherDto.Name.Trim()) || string.IsNullOrEmpty(teacherDto.MiddleName.Trim()))
            {
                throw new Exception("ФИО преподавателя должно быть полностью заполнено.");
            }

            string result = null;

            var teacherNew = teacherDto.ToTeacher();

            var exist = await _context.Teachers.AnyAsync(t =>
            t.FirstName == teacherNew.FirstName
            && t.Name == teacherNew.Name
            && t.MiddleName == teacherNew.MiddleName);

            if (exist)
            {
                result = "Такой преподаватель уже есть!";
            }
            else
            {
                if (versionId != null)
                {
                    var version = _context.Versions.FirstOrDefault(v => v.Id == versionId);

                    teacherNew.LessonNumbers = Enumerable.Range(0, version.MaxLesson).ToArray();

                    teacherNew.WeekDays = Enumerable.Range(0, version.UseSunday ? 7 : 6).ToArray();
                }
                else
                {
                    var version = _context.Versions.FirstOrDefault(v => v.IsActive);

                    teacherNew.LessonNumbers = Enumerable.Range(0, version.MaxLesson).ToArray();

                    teacherNew.WeekDays = Enumerable.Range(0, version.UseSunday ? 7 : 6).ToArray();
                }

                await _context.Teachers.AddAsync(teacherNew);

                await _context.SaveChangesAsync();
            }

            return result;
        }

        ///<inheritdoc/>
        public async Task<string> EditTeacherAsync(TeacherDto teacherDto)
        {
            if (string.IsNullOrEmpty(teacherDto.FirstName.Trim()) || string.IsNullOrEmpty(teacherDto.Name.Trim()) || string.IsNullOrEmpty(teacherDto.MiddleName.Trim()))
            {
                throw new Exception("ФИО преподавателя должно быть полностью заполнено.");
            }

            string result = null;

            var teacherNew = teacherDto.ToTeacher();

            var exist = await _context.Teachers.AnyAsync(t =>
            t.FirstName == teacherNew.FirstName
            && t.Name == teacherNew.Name
            && t.MiddleName == teacherNew.MiddleName
            && t.Id != teacherNew.Id);

            var teacherOld = await _context.Teachers.FirstOrDefaultAsync(t => t.Id == teacherDto.Id);

            if (exist)
            {
                result = "Такой преподватель уже есть!";
            }
            else
            {
                teacherOld.FirstName = teacherNew.FirstName;
                teacherOld.Name = teacherNew.Name;
                teacherOld.MiddleName = teacherNew.MiddleName;
                teacherOld.LessonNumbers = teacherNew.LessonNumbers;
                teacherOld.WeekDays = teacherNew.WeekDays;

                await _context.SaveChangesAsync();
            }

            return result;
        }

        ///<inheritdoc/>
        public async Task RemoveTeacherAsync(int id)
        {
            var teacher = await _context.Teachers.FirstOrDefaultAsync(s => s.Id == id);

            if (teacher != null)
            {
                _context.Teachers.Remove(teacher);

                await _context.SaveChangesAsync();
            }
        }
    }
}
