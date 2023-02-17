using DAL.Entities;
using DAL.Entities.Schedule;
using DTL.Dto.ScheduleDto;
using DTL.Mapping;
using Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BL.ServiceInterface;
using DAL;

namespace BL.Services
{
    public class SubdivisionService : ISubdivisionService
    {
        private readonly ScheduleHighSchoolDb _context;

        private readonly int _currentEmployeeId;

        public SubdivisionService(ScheduleHighSchoolDb context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _currentEmployeeId = int.Parse(httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == CustomConstants.EMPLOYEE_ID_CLAIM_TYPE).Value);
        }

        ///<inheritdoc/>
        public async Task<List<SubdivisionDto>> GetSubdivisionListAsync()
        {
            var subdivisonList = await _context.Subdivisions
                .AsNoTracking()
                .Select(s => s.ToDto<SubdivisionDto>())
                .ToListAsync();

            return subdivisonList;
        }

        ///<inheritdoc/>
        public async Task<List<SubdivisionDto>> GetCurrentEmployeeSubdivisionListAsync()
        {
            var currentEmployeeSubdivisionIds = await _context.EmployeeSubdivisions
                .AsNoTracking()
                .Where(es => es.EmployeeId == _currentEmployeeId)
                .Select(es => es.SubdivisionId)
                .ToListAsync();

            var subdivisionList = await _context.Subdivisions
                .AsNoTracking()
                .Where(es => currentEmployeeSubdivisionIds.Contains(es.Id))
                .Select(s => s.ToSubdivisionDto())
                .ToListAsync();

            return subdivisionList;
        }

        ///<inheritdoc/>
        public async Task<string> AddSubdivisionAsync(SubdivisionDto subdivisionDto)
        {
            if (string.IsNullOrEmpty(subdivisionDto.Name) || string.IsNullOrWhiteSpace(subdivisionDto.Name))
            {
                return null;
            }

            string result = null;

            var subdivison = await _context.Subdivisions.FirstOrDefaultAsync(s => s.Name == subdivisionDto.Name);

            if (subdivison == null)
            {
                var subdivision = subdivisionDto.ToEntity<Subdivision>();

                await _context.Subdivisions.AddAsync(subdivision);

                await _context.SaveChangesAsync();

                await _context.EmployeeSubdivisions.AddAsync(new EmployeeSubdivision { EmployeeId = 1, SubdivisionId = subdivision.Id });

                await _context.SaveChangesAsync();
            }
            else
            {
                result = "Такое подразделение уже есть!";
            }

            return result;
        }

        ///<inheritdoc/>
        public async Task<string> EditSubdivisionAsync(SubdivisionDto subdivisionDto)
        {
            if (string.IsNullOrEmpty(subdivisionDto.Name) || string.IsNullOrWhiteSpace(subdivisionDto.Name))
            {
                return null;
            }

            string result = null;

            var subdivison = await _context.Subdivisions.FirstOrDefaultAsync(s => s.Id == subdivisionDto.Id);

            if (subdivison != null)
            {
                subdivisionDto.Name = subdivisionDto.Name.Trim().ToUpper();

                var exist = await _context.Subdivisions.AnyAsync(s => s.Name == subdivisionDto.Name && s.Id != subdivisionDto.Id);

                if (exist)
                {
                    result = "Такое подразделение уже есть!";
                }
                else
                {
                    subdivison.Name = subdivisionDto.Name;

                    await _context.SaveChangesAsync();
                }
            }

            return result;
        }

        ///<inheritdoc/>
        public async Task RemoveSubdivisionAsync(int id)
        {
            var subdivison = await _context.Subdivisions.FirstOrDefaultAsync(s => s.Id == id);

            if (subdivison != null)
            {
                _context.Subdivisions.Remove(subdivison);

                await _context.SaveChangesAsync();
            }
        }
    }
}
