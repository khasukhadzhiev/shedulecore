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
    public class SubjectController : ControllerBase
    {
        private readonly ISubjectService _subjectService;

        public SubjectController(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        [HttpGet]
        [Route("GetSubjectList")]
        public async Task<IActionResult> GetSubjectList()
        {
            var subjects = await _subjectService.GetSubjectListAsync();

            return Ok(subjects);
        }

        [HttpPost]
        [Route("AddSubject")]
        public async Task<IActionResult> AddSubject(SubjectDto subjectDto)
        {
            var result = await _subjectService.AddSubjectAsync(subjectDto);

            return Ok(result);
        }

        [HttpPost]
        [Route("EditSubject")]
        public async Task<IActionResult> EditSubject(SubjectDto subjectDto)
        {
            var result = await _subjectService.EditSubjectAsync(subjectDto);

            return Ok(result);
        }

        [HttpGet]
        [Route("RemoveSubject")]
        public async Task<IActionResult> RemoveSubject(int id)
        {
            await _subjectService.RemoveSubjectAsync(id);

            return Ok();
        }
    }
}
