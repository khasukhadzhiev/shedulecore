using BL.ServiceInterface;
using DAL.Entities;
using DAL.Entities.Schedule;
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
    public class BuildingService : IBuildingService
    {
        private readonly ScheduleHighSchoolDb _context;

        public BuildingService(ScheduleHighSchoolDb context)
        {
            _context = context;
        }

        ///<inheritdoc/>
        public async Task<List<BuildingDto>> GetBuildingListAsync()
        {
            var buildingList = await _context.Building
                .AsNoTracking()
                .OrderBy(b => b.Name)
                .Select(s => s.ToDto<BuildingDto>())
                .ToListAsync();

            return buildingList;
        }

        ///<inheritdoc/>
        public async Task<string> AddBuildingAsync(BuildingDto buildingDto)
        {
            if (string.IsNullOrEmpty(buildingDto.Name.Trim()))
            {
                throw new Exception("Наименование корпуса не может быть пустым!");
            }

            string result = null;

            var buildingNew = buildingDto.ToEntity<Building>();

            var exist = await _context.Building.AnyAsync(b => b.Name == buildingDto.Name);

            if (exist)
            {
                result = "Такой корпус уже есть!";
            }
            else
            {
                await _context.Building.AddAsync(buildingNew);

                await _context.SaveChangesAsync();
            }

            return result;
        }

        ///<inheritdoc/>
        public async Task<string> EditBuildingAsync(BuildingDto buildingDto)
        {
            if (string.IsNullOrEmpty(buildingDto.Name.Trim()))
            {
                throw new Exception("Наименование корпуса не может быть пустым!");
            }

            string result = null;

            var buildingNew = buildingDto.ToEntity<Building>();

            var exist = await _context.Building.AnyAsync(t => t.Name == buildingNew.Name && t.Id != buildingNew.Id);

            if (exist)
            {
                result = "Такой корпус уже есть!";
            }
            else
            {
                var buildingOld = await _context.Building.FirstOrDefaultAsync(s => s.Id == buildingDto.Id);

                buildingOld.Name = buildingNew.Name;

                await _context.SaveChangesAsync();
            }

            return result;
        }

        ///<inheritdoc/>
        public async Task RemoveBuildingAsync(int id)
        {
            var building = await _context.Building.FirstOrDefaultAsync(b => b.Id == id);

            if (building != null)
            {
                _context.Building.Remove(building);

                await _context.SaveChangesAsync();
            }
        }
    }
}
