using DTL.Dto.ScheduleDto;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using BL.ServiceInterface;

namespace CoreAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SubdivisionController : ControllerBase
    {
        private readonly ISubdivisionService _subdivisionService;

        public SubdivisionController(ISubdivisionService subdivisionService)
        {
            _subdivisionService = subdivisionService;
        }

        [HttpGet]
        [Route("GetSubdivisionList")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin,employee")]
        public async Task<IActionResult> GetSubdivisionList()
        {
            var subdivisionList = await _subdivisionService.GetSubdivisionListAsync();

            return Ok(subdivisionList);
        }

        [HttpGet]
        [Route("GetCurrentEmployeeSubdivisionList")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin,employee")]
        public async Task<IActionResult> GetCurrentEmployeeSubdivisionList()
        {
            var currentEmployeeSubdivisionList = await _subdivisionService.GetCurrentEmployeeSubdivisionListAsync();

            return Ok(currentEmployeeSubdivisionList);
        }

        [HttpPost]
        [Route("AddSubdivision")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> AddSubdivision(SubdivisionDto subdivisionDto)
        {
            var result = await _subdivisionService.AddSubdivisionAsync(subdivisionDto);

            return Ok(result);
        }

        [HttpPost]
        [Route("EditSubdivision")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> EditSubdivision(SubdivisionDto subdivisionDto)
        {
            var result = await _subdivisionService.EditSubdivisionAsync(subdivisionDto);

            return Ok(result);
        }

        [HttpGet]
        [Route("RemoveSubdivision")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> RemoveSubdivision(int id)
        {
            await _subdivisionService.RemoveSubdivisionAsync(id);

            return Ok();
        }
    }
}