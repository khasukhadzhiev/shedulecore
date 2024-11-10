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
            var pdfStream = await _exportService.SaveScheduleToPdfAsync(saveFileModelDto);

            return File(pdfStream, "application/pdf");
        }

        [HttpPost]
        [Route("SaveTimetableReportingToPdf")]
        public async Task<FileStreamResult> SaveTimetableReportingToPdf(SavFileModelDto saveFileModelDto)
        {
            var pdfStream = await _exportService.SaveScheduleReportingToPdfAsync(saveFileModelDto);

            return File(pdfStream, "application/pdf");
        }
    }
}