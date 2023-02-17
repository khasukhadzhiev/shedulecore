using BL.ServiceInterface;
using DTL.Dto.ScheduleDto;
using DTL.Mapping;
using Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL;

namespace BL.Services
{
    public class StudyClassService : IStudyClassService
    {
        private readonly ScheduleHighSchoolDb _context;

        private readonly int _currentEmployeeId;

        public StudyClassService(ScheduleHighSchoolDb context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _currentEmployeeId = int.Parse(httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == CustomConstants.EMPLOYEE_ID_CLAIM_TYPE)?.Value ?? "-1");
        }

        ///<inheritdoc/>
        public async Task<List<StudyClassDto>> GetStudyClassListAsync()
        {
            var currentEmployeeSubdivisionIds = await _context.EmployeeSubdivisions
                .AsNoTracking()
                .Where(es => es.EmployeeId == _currentEmployeeId)
                .Select(es => es.SubdivisionId).ToListAsync();

            var studyClassList = await _context.StudyClasses
                .AsNoTracking()
                .Include(s => s.EducationForm)
                .Include(s => s.ClassShift)
                .Include(s => s.Subdivision)
                .Where(es => currentEmployeeSubdivisionIds.Contains(es.SubdivisionId))
                .OrderBy(s => s.Name.Length)
                .ThenBy(s => s.Name)
                .Select(s => s.ToStudyClassDto())
                .ToListAsync();

            return studyClassList;
        }

        ///<inheritdoc/>
        public async Task<List<List<StudyClassDto>>> GetStudyClassListBySubdivisionAsync(int subdivisionId)
        {
            var currentEmployeeSubdivisionIds = await _context.EmployeeSubdivisions
                .AsNoTracking()
                .Where(es => es.EmployeeId == _currentEmployeeId)
                .Select(es => es.SubdivisionId).ToListAsync();

            if (!currentEmployeeSubdivisionIds.Contains(subdivisionId))
            {
                throw new Exception("Сотрудник не привязан к переданному подразделению!");
            }

            List<List<StudyClassDto>> resultList = new List<List<StudyClassDto>>();

            var studyClassNameFirstLetterList = await _context.StudyClasses
                .AsNoTracking()
                .Include(s => s.Subdivision)
                .Where(s => s.SubdivisionId == subdivisionId)
                .Select(s => s.Name.Substring(0, 2))
                .Distinct()
                .ToListAsync();

            foreach (var letter in studyClassNameFirstLetterList)
            {
                var studyClassList = await _context.StudyClasses
                    .AsNoTracking()
                    .Include(s => s.EducationForm)
                    .Include(s => s.ClassShift)
                    .Include(s => s.Subdivision)
                    .Where(s => s.SubdivisionId == subdivisionId && s.Name.Substring(0, 2) == letter)
                    .OrderBy(s => s.Name)
                    .Select(s => s.ToStudyClassDto())
                    .ToListAsync();

                resultList.Add(studyClassList);
            }

            return resultList;
        }

        ///<inheritdoc/>
        public async Task<string> AddStudyClassAsync(StudyClassDto studyClassDto)
        {
            string result = null;

            if (studyClassDto.Subdivision != null)
            {
                studyClassDto.SubdivisionId = studyClassDto.Subdivision.Id;

                studyClassDto.Subdivision = null;
            }

            var studyClass = studyClassDto.ToStudyClass();

            var exist = await _context.StudyClasses.AnyAsync(s => s.Name == studyClass.Name && s.SubdivisionId == studyClass.SubdivisionId);

            if (exist)
            {
                result = "Такая группа уже есть!";
            }
            else
            {
                await _context.StudyClasses.AddAsync(studyClass);

                await _context.SaveChangesAsync();
            }

            return result;
        }

        ///<inheritdoc/>
        public async Task<string> EditStudyClassAsync(StudyClassDto studyClassDto)
        {
            string result = null;

            if (studyClassDto.Subdivision != null)
            {
                studyClassDto.SubdivisionId = studyClassDto.Subdivision.Id;

                studyClassDto.Subdivision = null;
            }

            var studyClassNew = studyClassDto.ToStudyClass();

            var studyClassOld = await _context.StudyClasses.FirstOrDefaultAsync(s => s.Id == studyClassDto.Id);

            var exist = await _context.StudyClasses.AnyAsync(s =>
            s.Name == studyClassNew.Name && s.Id != studyClassNew.Id);

            if (exist)
            {
                result = "Такая группа уже есть!";
            }
            else
            {
                studyClassOld.Name = studyClassNew.Name;
                studyClassOld.StudentsCount = studyClassNew.StudentsCount;
                studyClassOld.ClassShiftId = studyClassNew.ClassShiftId;
                studyClassOld.EducationFormId = studyClassNew.EducationFormId;
                studyClassOld.SubdivisionId = studyClassNew.SubdivisionId;

                await _context.SaveChangesAsync();
            }

            return result;
        }

        ///<inheritdoc/>
        public async Task RemoveStudyClassAsync(int id)
        {
            var studyClass = await _context.StudyClasses.FirstOrDefaultAsync(s => s.Id == id);

            if (studyClass != null)
            {
                _context.StudyClasses.Remove(studyClass);

                await _context.SaveChangesAsync();
            }
        }

        ///<inheritdoc/>
        public async Task<List<EducationFormDto>> GetEducationFormListAsync()
        {
            var educationFormList = await _context.EducationForms
                .AsNoTracking()
                .Select(e => e.ToEducationFormDto())
                .ToListAsync();

            return educationFormList;
        }

        ///<inheritdoc/>
        public async Task<List<ClassShiftDto>> GetClassShiftListAsync()
        {
            var classShiftList = await _context.ClassShifts
                .AsNoTracking()
                .Select(c => c.ToClassShiftDto())
                .ToListAsync();

            return classShiftList;
        }
    }
}
