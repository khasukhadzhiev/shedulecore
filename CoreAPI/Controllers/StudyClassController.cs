using BL.ServiceInterface;
using DTL.Dto.ScheduleDto;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CoreAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "employee")]
    [Route("api/[controller]")]
    [ApiController]
    public class StudyClassController : ControllerBase
    {
        private readonly IStudyClassService _studyClassService;

        public StudyClassController(IStudyClassService studyClassService)
        {
            _studyClassService = studyClassService;
        }

        [HttpGet]
        [Route("GetStudyClassList")]
        public async Task<IActionResult> GetStudyClassList()
        {
            var studyClassList = await _studyClassService.GetStudyClassListAsync();

            return Ok(studyClassList);
        }

        [HttpGet]
        [Route("GetStudyClassListBySubdivision")]
        public async Task<IActionResult> GetStudyClassListBySubdivision(int subdivisionId)
        {
            var studyClassList = await _studyClassService.GetStudyClassListBySubdivisionAsync(subdivisionId);

            return Ok(studyClassList);
        }

        [HttpPost]
        [Route("AddStudyClass")]
        public async Task<IActionResult> AddStudyClass(StudyClassDto studyClassDto)
        {
            var result = await _studyClassService.AddStudyClassAsync(studyClassDto);

            return Ok(result);
        }

        [HttpPost]
        [Route("EditStudyClass")]
        public async Task<IActionResult> EditStudyClass(StudyClassDto studyClassDto)
        {
            var result = await _studyClassService.EditStudyClassAsync(studyClassDto);

            return Ok(result);
        }

        [HttpGet]
        [Route("RemoveStudyClass")]
        public async Task<IActionResult> RemoveStudyClass(int id)
        {
            await _studyClassService.RemoveStudyClassAsync(id);

            return Ok();
        }

        [HttpGet]
        [Route("GetEducationFormList")]
        public async Task<IActionResult> GetEducationFormList()
        {
            var educationFormList = await _studyClassService.GetEducationFormListAsync();

            return Ok(educationFormList);
        }

        [HttpGet]
        [Route("GetClassShiftList")]
        public async Task<IActionResult> GetClassShiftList()
        {
            var classShiftList = await _studyClassService.GetClassShiftListAsync();

            return Ok(classShiftList);
        }
    }
}