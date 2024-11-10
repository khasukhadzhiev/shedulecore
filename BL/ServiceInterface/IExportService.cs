using DTL.Dto;
using System.IO;
using System.Threading.Tasks;

namespace BL.ServiceInterface
{
    public interface IExportService
    {
        /// <summary>
        /// Сохранить расписание в Pdf
        /// </summary>
        /// <param name="savFileModelDto">Объект с данными для генерации PDF</param>
        /// <returns></returns>
        Task<Stream> SaveTimetableToPdfAsync(SavFileModelDto savFileModelDto);

        /// <summary>
        /// Сохранить расписание отчетностей в Pdf
        /// </summary>
        /// <param name="savFileModelDto">Объект с данными для генерации PDF</param>
        /// <returns></returns>
        Task<Stream> SaveTimetableReportingToPdfAsync(SavFileModelDto savFileModelDto);

        /// <summary>
        /// Сохранить расписание в xlsx
        /// </summary>
        /// <param name="savFileModelDto">Объект с данными для генерации XLSX</param>
        /// <returns></returns>
        Task<Stream> SaveTimetableToXlsxAsync(SavFileModelDto savFileModelDto);

        /// <summary>
        /// Сохранить отчетность в xlsx
        /// </summary>
        /// <param name="savFileModelDto">Объект с данными для генерации XLSX</param>
        /// <returns></returns>
        Task<Stream> SaveTimetableReportingToXlsxAsync(SavFileModelDto savFileModelDto);
    }
}
