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
    public class TeacherController : ControllerBase
    {
        private ITeacherService _teacherService;

        public TeacherController(ITeacherService teacherService)
        {
            _teacherService = teacherService;
        }

        [HttpGet]
        [Route("GetTeacherList")]
        public async Task<IActionResult> GetTeacherList()
        {
            var teachersList = await _teacherService.GetTeacherListAsync();

            return Ok(teachersList);
        }

        [HttpPost]
        [Route("AddTeacher")]
        public async Task<IActionResult> AddTeacher(TeacherDto teacherDto)
        {
            var result = await _teacherService.AddTeacherAsync(teacherDto);

            return Ok(result);
        }

        [HttpPost]
        [Route("EditTeacher")]
        public async Task<IActionResult> EditTeacher(TeacherDto teacherDto)
        {
            var result = await _teacherService.EditTeacherAsync(teacherDto);

            return Ok(result);
        }

        [HttpGet]
        [Route("RemoveTeacher")]
        public async Task<IActionResult> RemoveTeacher(int id)
        {
            await _teacherService.RemoveTeacherAsync(id);

            return Ok();
        }
    }
}