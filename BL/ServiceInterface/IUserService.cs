using DTL.Dto.ScheduleDto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BL.ServiceInterface
{
    public interface IUserService
    {
        /// <summary>
        /// Получить расписание
        /// </summary>
        /// <param name="queryString">Строка для поиска</param>
        /// <returns>Расписание</returns>
        Task<List<QueryScheduleDto>> GetScheduleAsync(string queryString);

        /// <summary>
        /// Получить расписание отчетностей
        /// </summary>
        /// <param name="queryString">Строка для поиска</param>
        /// <returns>Расписание</returns>
        Task<List<QueryScheduleReportingDto>> GetReportingScheduleAsync(string queryString);

        /// <summary>
        /// Получить список вариантов для поиска
        /// </summary>
        /// <returns></returns>
        Task<List<string>> GetQueryOptionListAsync();
    }
}
