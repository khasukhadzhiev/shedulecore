using Infrastructure;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace BL.ServiceInterface
{
    public interface IImportService
    {
        /// <summary>
        /// Импортировать файл с данными расписания
        /// </summary>
        /// <param name="file">Импортируемый файл</param>
        /// <returns></returns>
        Task ImportScheduleDataAsync(IFormFile file, int scheduleVersionId);

        /// <summary>
        /// Импортировать список аудиторий
        /// </summary>
        /// <param name="file">Импортируемый файл</param>
        /// <returns></returns>
        Task ImportClassroomListAsync(IFormFile file, int scheduleVersionId);

        /// <summary>
        /// Очистить историю импорта
        /// </summary>
        /// <returns></returns>
        Task RemoveImportProgressAsync();

        /// <summary>
        /// Получить прогресс импорта данных
        /// </summary>
        /// <returns></returns>
        Task<ImportProgress> GetImportProgressAsync();
    }
}
