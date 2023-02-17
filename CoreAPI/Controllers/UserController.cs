using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using BL.ServiceInterface;

namespace CoreAPI.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Route("GetTimetable")]
        public async Task<IActionResult> GetTimetable(string queryString)
        {
            var timetable = await _userService.GetScheduleAsync(queryString);

            return Ok(timetable);
        }

        [HttpGet]
        [Route("GetReportingTimetable")]
        public async Task<IActionResult> GetReportingTimetable(string queryString)
        {
            var timetableReporting = await _userService.GetReportingScheduleAsync(queryString);

            return Ok(timetableReporting);
        }

        [HttpGet]
        [Route("GetQueryOptionList")]
        public async Task<IActionResult> GetQueryOptionList()
        {
            var timetable = await _userService.GetQueryOptionListAsync();

            return Ok(timetable);
        }

    }
}