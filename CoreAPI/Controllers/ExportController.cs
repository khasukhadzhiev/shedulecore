using DTL.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using BL.ServiceInterface;

namespace CoreAPI.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class ExportController : ControllerBase
    {
        private IExportService _exportService;

        public ExportController(IExportService exportService)
        {
            _exportService = exportService;
        }

        [HttpPost]
        [Route("SaveTimetableToPdf")]
        public async Task<FileStreamResult> SaveTimetableToPdf(SavFileModelDto saveFileModelDto)
        {
            var pdfStream = await _exportService.SaveTimetableToPdfAsync(saveFileModelDto);

            return File(pdfStream, "application/pdf");
        }

        [HttpPost]
        [Route("SaveTimetableReportingToPdf")]
        public async Task<FileStreamResult> SaveTimetableReportingToPdf(SavFileModelDto saveFileModelDto)
        {
            var pdfStream = await _exportService.SaveTimetableReportingToPdfAsync(saveFileModelDto);

            return File(pdfStream, "application/pdf");
        }

        [HttpPost]
        [Route("SaveTimetableToXlsx")]
        public async Task<FileStreamResult> SaveTimetableToXlsx(SavFileModelDto saveFileModelDto)
        {
            if (saveFileModelDto.Name.Length > 150)
            {
                saveFileModelDto.Name = saveFileModelDto.Name.Substring(0, 150);
            }

            var xlsxStream = await _exportService.SaveTimetableToXlsxAsync(saveFileModelDto);

            return File(xlsxStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

        }

        [HttpPost]
        [Route("SaveTimetableReportingToXlsx")]
        public async Task<FileStreamResult> SaveTimetableReportingToXlsx(SavFileModelDto saveFileModelDto)
        {
            if (saveFileModelDto.Name.Length > 150)
            {
                saveFileModelDto.Name = saveFileModelDto.Name.Substring(0, 150);
            }

            var xlsxStream = await _exportService.SaveTimetableReportingToXlsxAsync(saveFileModelDto);

            return File(xlsxStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

        }
    }
}