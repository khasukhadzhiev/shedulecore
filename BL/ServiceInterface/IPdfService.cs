using DTL.Dto;
using System.IO;
using System.Threading.Tasks;

namespace BL.ServiceInterface
{
    public interface IPdfService
    {
        /// <summary>
        /// Сохранить расписание в Pdf
        /// </summary>
        /// <param name="pdfSaveModelDto">Объект с данными для генерации PDF</param>
        /// <returns></returns>
        Task<Stream> SaveScheduleToPdfAsync(PdfSaveModelDto pdfSaveModelDto);

        /// <summary>
        /// Сохранить расписание отчетностей в Pdf
        /// </summary>
        /// <param name="pdfSaveModelDto">Объект с данными для генерации PDF</param>
        /// <returns></returns>
        Task<Stream> SaveScheduleReportingToPdfAsync(PdfSaveModelDto pdfSaveModelDto);


    }
}
