using DTL.Dto.ScheduleDto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BL.ServiceInterface
{
    public interface IStudyClassReportingService
    {

        /// <summary>
        /// Получить весь список видов отчетностей
        /// </summary>
        /// <returns></returns>
        public Task<List<ReportingTypeDto>> GetReportingTypeListAsync();

        /// <summary>
        /// Получить весь список отчетностей группы по Id группы
        /// </summary>
        /// <param name="studyClassId"></param>
        /// <returns></returns>
        public Task<List<StudyClassReportingDto>> GetStudyClassReportingListAsync(int studyClassId, int versionId);

        /// <summary>
        /// Добавить отчетность группы по дисциплине
        /// </summary>
        /// <param name="studyClassReportingDto"></param>
        /// <returns></returns>
        public Task<string> AddStudyClassReportingAsync(StudyClassReportingDto studyClassReportingDto);

        /// <summary>
        /// Изменить отчетность группы по дисциплине
        /// </summary>
        /// <param name="studyClassReportingDto"></param>
        /// <returns></returns>
        public Task<string> EditStudyClassReportingAsync(StudyClassReportingDto studyClassReportingDto);

        /// <summary>
        /// Удалить отчетность группы по дисциплине по Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task RemoveStudyClassReportingAsync(int id);
    }
}
