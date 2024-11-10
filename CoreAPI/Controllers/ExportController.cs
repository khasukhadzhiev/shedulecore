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
            var pdfStream = await _exportService.SaveTimetableToXlsxAsync(saveFileModelDto);

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
            var xlsxStream = await _exportService.SaveTimetableToXlsxAsync(saveFileModelDto);

            return File(xlsxStream, "application/vnd.ms-excel");
        }
    }
}