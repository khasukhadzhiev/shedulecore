using BL.ServiceInterface;
using DTL.Dto.ScheduleDto;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "employee")]
    [Route("api/[controller]")]
    [ApiController]
    public class ClassroomController : ControllerBase
    {
        private readonly IClassroomService _classroomService;

        public ClassroomController(IClassroomService classroomService)
        {
            _classroomService = classroomService;
        }

        [HttpGet]
        [Route("GetClassroomList")]
        public async Task<IActionResult> GetClassroomList()
        {
            var classrooms = await _classroomService.GetClassroomListAsync();

            return Ok(classrooms);
        }

        [HttpGet]
        [Route("GetClassroomTypeList")]
        public async Task<IActionResult> GetClassroomTypeList()
        {
            var classroomTypes = await _classroomService.GetClassroomTypeListAsync();

            return Ok(classroomTypes);
        }

        [HttpPost]
        [Route("AddClassroom")]
        public async Task<IActionResult> AddClassroom(ClassroomDto classroomDto)
        {
            var result = await _classroomService.AddClassroomAsync(classroomDto);

            return Ok(result);
        }

        [HttpPost]
        [Route("EditClassroom")]
        public async Task<IActionResult> EditClassroom(ClassroomDto classroomDto)
        {
            var result = await _classroomService.EditClassroomAsync(classroomDto);

            return Ok(result);
        }

        [HttpGet]
        [Route("RemoveClassroom")]
        public async Task<IActionResult> RemoveClassroom(int id)
        {
            await _classroomService.RemoveClassroomAsync(id);

            return Ok();
        }

        [HttpPost]
        [Route("SetClassroomByLesson")]
        public async Task<IActionResult> SetClassroomByLesson(int lessonId, int classroomId)
        {
            await _classroomService.SetClassroomByLessonAsync(lessonId, classroomId);

            return Ok();
        }

        [HttpPost]
        [Route("GetWarningsByClassroom")]
        public async Task<IActionResult> GetWarningsByClassroom(int lessonId)
        {
            var result = await _classroomService.GetWarningsByClassroomAsync(lessonId);

            return Ok(result);
        }

        [HttpPost]
        [Route("GetTimetableByClassrooms")]
        public async Task<IActionResult> GetTimetableByClassrooms(List<ClassroomDto> selectedClassroomListDto, int versionId)
        {
            var timetable = await _classroomService.GetScheduleByClassroomsAsync(selectedClassroomListDto, versionId);

            return Ok(timetable);
        }
    }
}