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
    public class StudyClassReportingService : IStudyClassReportingService
    {
        private readonly ScheduleHighSchoolDb _context;

        public StudyClassReportingService(ScheduleHighSchoolDb context)
        {
            _context = context;
        }

        ///<inheritdoc/>
        public async Task<List<ReportingTypeDto>> GetReportingTypeListAsync()
        {
            var reportingTypeList = await _context.ReportingType
                .AsNoTracking()
                .AsQueryable()
                .Select(r => r.ToReportingTypeDto())
                .ToListAsync();

            return reportingTypeList;
        }

        ///<inheritdoc/>
        public async Task<List<StudyClassReportingDto>> GetStudyClassReportingListAsync(int studyClassId, int versionId)
        {
            var studyClassReportingList = await _context.StudyClassReporting
                .AsNoTracking()
                .AsQueryable()
                .Include(r => r.Subject)
                .Include(r => r.Teacher)
                .Include(r => r.ReportingType)
                .Include(r => r.StudyClass)
                .Include(r => r.Classroom)
                    .ThenInclude(c => c.Building)
                .Where(r => r.StudyClassId == studyClassId && r.VersionId == versionId)
                .Select(r => r.ToStudyClassReportingDto())
                .ToListAsync();

            return studyClassReportingList;
        }

        ///<inheritdoc/>
        public async Task<string> AddStudyClassReportingAsync(StudyClassReportingDto studyClassReportingDto)
        {
            string result = null;

            var exist = await _context.StudyClassReporting
                .AnyAsync(r =>
                r.StudyClassId == studyClassReportingDto.StudyClassId
                && r.SubjectId == studyClassReportingDto.SubjectId
                && r.VersionId == studyClassReportingDto.VersionId);

            if (exist)
            {
                result = "Отчетность по указанной дисциплине для указанной группы уже добавлена!";
            }
            else
            {
                var studyClassReporting = studyClassReportingDto.ToStudyClassReporting();

                await _context.StudyClassReporting.AddAsync(studyClassReporting);

                await _context.SaveChangesAsync();
            }

            return result;
        }

        ///<inheritdoc/>
        public async Task<string> EditStudyClassReportingAsync(StudyClassReportingDto studyClassReportingDto)
        {
            string result = null;

            var exist = await _context.StudyClassReporting.AnyAsync(r => r.Id == studyClassReportingDto.Id);

            if (exist)
            {
                var studyClassReporting = await _context.StudyClassReporting.FirstOrDefaultAsync(r => r.Id == studyClassReportingDto.Id);

                studyClassReporting.StudyClassId = studyClassReportingDto.StudyClassId;
                studyClassReporting.SubjectId = studyClassReportingDto.SubjectId;
                studyClassReporting.TeacherId = studyClassReportingDto.TeacherId;
                studyClassReporting.ReportingTypeId = studyClassReportingDto.ReportingTypeId;
                studyClassReporting.VersionId = studyClassReportingDto.VersionId;
                studyClassReporting.ClassroomId = studyClassReportingDto?.ClassroomId;

                if (string.IsNullOrEmpty(studyClassReportingDto?.Date))
                {
                    studyClassReporting.Date = null;
                }
                else
                {
                    studyClassReporting.Date = DateTime.Parse(studyClassReportingDto?.Date);
                }

                await _context.SaveChangesAsync();
            }
            else
            {
                result = "Указанная отчетность не найдена!";
            }

            return result;
        }

        ///<inheritdoc/>
        public async Task RemoveStudyClassReportingAsync(int id)
        {
            var exist = await _context.StudyClassReporting.AnyAsync(r => r.Id == id);

            if (exist)
            {
                var studyClassReporting = await _context.StudyClassReporting.FirstOrDefaultAsync(r => r.Id == id);

                _context.Remove(studyClassReporting);

                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Указанная отчетность не найдена!");
            }
        }
    }
}
