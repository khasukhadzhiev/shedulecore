using BL.ServiceInterface;
using DTL.Dto.ScheduleDto;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin, employee")]
    public class StudyClassReportingController : ControllerBase
    {
        private readonly IStudyClassReportingService _studyClassReportingService;

        public StudyClassReportingController(IStudyClassReportingService studyClassReportingService)
        {
            _studyClassReportingService = studyClassReportingService;
        }

        [HttpGet]
        [Route("GetReportingTypeList")]
        public async Task<IActionResult> GetReportingTypeList(int studyClassId)
        {
            var reportingTypeList = await _studyClassReportingService.GetReportingTypeListAsync();

            return Ok(reportingTypeList);
        }

        [HttpGet]
        [Route("GetStudyClassReportingList")]
        public async Task<IActionResult> GetStudyClassReportingList(int studyClassId, int versionId)
        {
            var studyClassReportingList = await _studyClassReportingService.GetStudyClassReportingListAsync(studyClassId, versionId);

            return Ok(studyClassReportingList);
        }

        [HttpPost]
        [Route("AddStudyClassReporting")]
        public async Task<IActionResult> AddStudyClassReporting(StudyClassReportingDto studyClassReportingDto)
        {
            var studyClassReporting = await _studyClassReportingService.AddStudyClassReportingAsync(studyClassReportingDto);

            return Ok(studyClassReporting);
        }

        [HttpPost]
        [Route("EditStudyClassReporting")]
        public async Task<IActionResult> EditStudyClassReporting(StudyClassReportingDto studyClassReportingDto)
        {
            var studyClassReporting = await _studyClassReportingService.EditStudyClassReportingAsync(studyClassReportingDto);

            return Ok(studyClassReporting);
        }

        [HttpGet]
        [Route("RemoveStudyClassReporting")]
        public async Task<IActionResult> RemoveStudyClassReporting(int id)
        {
            await _studyClassReportingService.RemoveStudyClassReportingAsync(id);

            return Ok();
        }
    }
}
