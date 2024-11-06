using BL.ServiceInterface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CoreAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class ImportController : ControllerBase
    {
        private readonly IImportService _importService;

        public ImportController(IImportService importService)
        {
            _importService = importService; 
        }

        [HttpPost]
        [Route("ImportTimetableData")]
        public async Task<IActionResult> ImportTimetableData(IFormFile file, int versionId)
        {
            await _importService.ImportScheduleDataAsync(file, versionId);

            return Ok();
        }

        [HttpPost]
        [Route("ImportClassroomList")]
        public async Task<IActionResult> ImportClassroomList(IFormFile file, int versionId)
        {
            await _importService.ImportClassroomListAsync(file, versionId);

            return Ok();
        }

        [HttpGet]
        [Route("GetImportProgress")]
        public async Task<IActionResult> GetImportProgress()
        {
            var importProgress = await _importService.GetImportProgressAsync();

            return Ok(importProgress);
        }

        [HttpGet]
        [Route("RemoveImportProgress")]
        public async Task<IActionResult> RemoveImportProgress()
        {
            await _importService.RemoveImportProgressAsync();

            return Ok();
        }
    }
}