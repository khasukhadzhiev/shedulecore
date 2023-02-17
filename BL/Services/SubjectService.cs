using BL.ServiceInterface;
using DTL.Dto.ScheduleDto;
using DTL.Mapping;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL;

namespace BL.Services
{
    public class SubjectService : ISubjectService
    {
        private readonly ScheduleHighSchoolDb _context;

        public SubjectService(ScheduleHighSchoolDb context)
        {
            _context = context;
        }

        ///<inheritdoc/>
        public async Task<List<SubjectDto>> GetSubjectListAsync()
        {
            var subjectList = await _context.Subjects
                .AsNoTracking()
                .OrderBy(s => s.Name)
                .Select(s => s.ToSubjectDto())
                .ToListAsync();

            return subjectList;
        }

        ///<inheritdoc/>
        public async Task<string> AddSubjectAsync(SubjectDto subjectDto)
        {
            string result = null;

            var subject = subjectDto.ToSubject();

            var exist = await _context.Subjects.AnyAsync(t => t.Name == subject.Name);

            if (exist)
            {
                result = "Такая дисциплина уже есть!";
            }
            else
            {
                await _context.Subjects.AddAsync(subject);

                await _context.SaveChangesAsync();
            }

            return result;
        }

        ///<inheritdoc/>
        public async Task<string> EditSubjectAsync(SubjectDto subjectDto)
        {
            string result = null;

            var subjectNew = subjectDto.ToSubject();

            var exist = await _context.Subjects.AnyAsync(s => s.Name == subjectNew.Name && s.Id != subjectDto.Id);

            var subjectOld = await _context.Subjects.FirstOrDefaultAsync(s => s.Id == subjectDto.Id);

            if (exist)
            {
                result = "Такая дисциплина уже есть!";
            }
            else
            {
                subjectOld.Name = subjectNew.Name;

                await _context.SaveChangesAsync();
            }

            return result;
        }

        ///<inheritdoc/>
        public async Task RemoveSubjectAsync(int id)
        {
            var subject = await _context.Subjects.FirstOrDefaultAsync(s => s.Id == id);

            if (subject != null)
            {
                _context.Subjects.Remove(subject);

                await _context.SaveChangesAsync();
            }
        }
    }
}
