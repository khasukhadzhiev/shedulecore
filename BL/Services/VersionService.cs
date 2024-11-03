using DTL.Dto;
using DTL.Mapping;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BL.ServiceInterface;
using DAL;
using DAL.Entities.Schedule;
using DAL.Entities;

namespace BL.Services
{
    public class VersionService : IVersionService
    {
        private readonly ScheduleHighSchoolDb _context;

        public VersionService(ScheduleHighSchoolDb context)
        {
            _context = context;
        }

        ///<inheritdoc/>
        public async Task<List<VersionDto>> GetVersionListAsync()
        {
            var versionList = await _context.Versions
                .AsNoTracking()
                .OrderBy(v => v.Name)
                .Select(v => v.ToVersionDto()).ToListAsync();

            return versionList;
        }

        ///<inheritdoc/>
        public async Task<VersionDto> GetVersionAsync(int versionId)
        {
            var version = await _context.Versions
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.Id == versionId);

            return version.ToVersionDto();
        }

        ///<inheritdoc/>
        public async Task<VersionDto> GetActiveVersionAsync()
        {
            var version = await _context.Versions
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.IsActive);

            return version.ToVersionDto();
        }

        ///<inheritdoc/>
        public async Task<string> AddVersionAsync(VersionDto versionDto)
        {
            if (string.IsNullOrEmpty(versionDto.Name.Trim()))
            {
                throw new Exception("Наименование должно быть заполнено.");
            }

            string result = null;

            if (versionDto.IsActive)
            {
                var versionList = await _context.Versions.Where(s => s.IsActive).ToListAsync();

                foreach (var item in versionList)
                {
                    item.IsActive = false;
                }
            }

            var version = versionDto.ToVersion();

            var exist = await _context.Versions.AnyAsync(t => t.Name == version.Name);

            if (exist)
            {
                result = "Такая версия расписания уже есть!";
            }
            else
            {
                version.MaxLesson = 6;

                await _context.Versions.AddAsync(version);

                await _context.SaveChangesAsync();
            }

            return result;
        }

        ///<inheritdoc/>
        public async Task<string> EditVersionAsync(VersionDto versionDto)
        {
            if (string.IsNullOrEmpty(versionDto.Name.Trim()) || versionDto.MaxLesson < 2)
            {
                throw new Exception("Наименование должно быть заполнено и макс. число занятий в день должно быть больше 1.");
            }

            string result = null;

            var isActiveExists = await _context.Versions.AnyAsync(s => s.IsActive && s.Id != versionDto.Id);

            if (!isActiveExists && versionDto.IsActive == false)
            {
                throw new Exception("Нельзя деактивировать все версии расписания!");
            }

            if (versionDto.IsActive)
            {
                var versionList = await _context.Versions.Where(v => v.IsActive).ToListAsync();

                foreach (var item in versionList)
                {
                    item.IsActive = false;
                }
            }

            var scheduleVersion = versionDto.ToVersion();

            var exist = await _context.Versions.AnyAsync(s => s.Name == versionDto.Name && s.Id != versionDto.Id);

            if (exist)
            {
                result = "Такая версия расписания уже есть!";
            }
            else
            {
                var version = await _context.Versions.FirstOrDefaultAsync(v => v.Id == versionDto.Id);

                CalculateRowIndex(version, versionDto);

                version.IsActive = versionDto.IsActive;
                version.Name = versionDto.Name;
                version.MaxLesson = versionDto.MaxLesson;
                version.UseSunday = versionDto.UseSunday;
                version.UseSubWeek = versionDto.UseSubWeek;
                version.UseSubClass = versionDto.UseSubClass;
                version.ShowEducationForm = versionDto.ShowEducationForm;
                version.ShowClassShift = versionDto.ShowClassShift;
                version.ShowReportingIds = versionDto.ShowReportingIds;

                await _context.SaveChangesAsync();
            }

            return result;
        }

        ///<inheritdoc/>
        public async Task RemoveVersionAsync(int id)
        {
            var versionCount = await _context.Versions.CountAsync();

            if (versionCount < 2)
            {
                throw new Exception("Нельзя удалить единственную версию расписания!");
            }

            var removingThisActive = await _context.Versions.Where(v => v.IsActive && v.Id != id).CountAsync();

            if (removingThisActive < 1)
            {
                throw new Exception("Нельзя удалить активную версию расписания!");
            }

            var version = await _context.Versions.FirstOrDefaultAsync(v => v.Id == id);

            _context.Versions.Remove(version);

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Пересчет позиции занятия при измении количества занятий в день или деления на недели
        /// </summary>
        /// <param name="versionOld"></param>
        /// <param name="versionNew"></param>
        private void CalculateRowIndex(DAL.Entities.Version versionOld, VersionDto versionNew) 
        {
            if (versionOld.MaxLesson != versionNew.MaxLesson || versionOld.UseSubWeek != versionNew.UseSubWeek)
            {
                var setedLessonListByVersion = _context.Lessons
                    .Include(l => l.Version)
                    .Where(l => l.VersionId == versionOld.Id)
                    .Where(l => l.RowIndex != null)
                    .ToList();

                var isToLowCountMaxLesson = setedLessonListByVersion.Any(l => l.LessonNumber+1 > versionNew.MaxLesson);

                if (isToLowCountMaxLesson)
                {
                    throw new Exception("Нельзя ставить число занятий в день, меньше чем уже поставлено занятие в сетке расписания!");
                }

                foreach (var lesson in setedLessonListByVersion)
                {
                    int rowIndex = lesson.RowIndex.Value;

                    int lessonDay = lesson.LessonDay.Value;

                    int lessonNumber = lesson.LessonNumber.Value;

                    int? lessonWeek = lesson.WeekNumber;

                    lesson.RowIndex = versionNew.UseSubWeek ? lessonDay * versionNew.MaxLesson * 2 : lessonDay * versionNew.MaxLesson;

                    lesson.RowIndex += versionNew.UseSubWeek ? lessonNumber * 2 : lessonNumber;

                    if (lesson.WeekNumber != null)
                    {
                        lesson.RowIndex += lessonWeek.Value == 1 ? 0 : 1;
                    }
                }
            }
        }
    }
}
