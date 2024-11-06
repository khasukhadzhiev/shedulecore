using BL.ServiceInterface;
using DAL;
using DTL.Dto.ScheduleDto;
using DTL.Mapping;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services
{
    public class ClassroomTypeService : IClassroomTypeService
    {
        private readonly ScheduleHighSchoolDb _context;

        public ClassroomTypeService(ScheduleHighSchoolDb context)
        {
            _context = context;
        }

        ///<inheritdoc/>
        public async Task<List<ClassroomTypeDto>> GetClassroomTypeListAsync()
        {
            var classroomTypeList = await _context.ClassroomTypes
                .AsNoTracking()
                .OrderBy(s => s.Name)
                .Select(s => s.ToClassroomTypeDto())
                .ToListAsync();

            return classroomTypeList;
        }

        ///<inheritdoc/>
        public async Task<string> AddClassroomTypeAsync(ClassroomTypeDto classroomTypeDto)
        {
            if (string.IsNullOrEmpty(classroomTypeDto.Name.Trim()))
            {
                throw new Exception("Наименование типа аудитории должно быть заполнено.");
            }

            string result = null;

            var exist = await _context.ClassroomTypes.AnyAsync(c => c.Name == classroomTypeDto.Name);

            if (exist)
            {
                result = "Такой тип аудитория уже есть!";
            }
            else
            {
                await _context.ClassroomTypes.AddAsync(classroomTypeDto.ToClassroomType());

                await _context.SaveChangesAsync();
            }

            return result;
        }

        ///<inheritdoc/>
        public async Task<string> EditClassroomTypeAsync(ClassroomTypeDto classroomTypeDto)
        {
            if (string.IsNullOrEmpty(classroomTypeDto.Name.Trim()))
            {
                throw new Exception("Наименование типа аудитории должно быть заполнено.");
            }

            string result = null;

            var exist = await _context.Classrooms.AnyAsync(c => c.Name == classroomTypeDto.Name);

            if (exist)
            {
                result = "Такой тип аудитории уже есть!";
            }
            else
            {
                var classroomTypeOld = await _context.ClassroomTypes.FirstOrDefaultAsync(s => s.Id == classroomTypeDto.Id);

                classroomTypeOld.Name = classroomTypeDto.Name.Trim().ToUpper();

                await _context.SaveChangesAsync();
            }

            return result;
        }

        ///<inheritdoc/>
        public async Task RemoveClassroomTypeAsync(int id)
        {
            var classroomType = await _context.ClassroomTypes.FirstOrDefaultAsync(s => s.Id == id);

            if (classroomType != null)
            {
                _context.ClassroomTypes.Remove(classroomType);

                await _context.SaveChangesAsync();
            }
        }
    }
}
