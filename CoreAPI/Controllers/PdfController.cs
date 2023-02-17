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
    public class PdfController : ControllerBase
    {
        private IPdfService _pdfService;

        public PdfController(IPdfService pdfService)
        {
            _pdfService = pdfService;
        }

        [HttpPost]
        [Route("SaveTimetableToPdf")]
        public async Task<FileStreamResult> SaveTimetableToPdf(PdfSaveModelDto pdfSaveModelDto)
        {
            var pdfStream = await _pdfService.SaveScheduleToPdfAsync(pdfSaveModelDto);

            return File(pdfStream, "application/pdf");
        }

        [HttpPost]
        [Route("SaveTimetableReportingToPdf")]
        public async Task<FileStreamResult> SaveTimetableReportingToPdf(PdfSaveModelDto pdfSaveModelDto)
        {
            var pdfStream = await _pdfService.SaveScheduleReportingToPdfAsync(pdfSaveModelDto);

            return File(pdfStream, "application/pdf");
        }
    }
}