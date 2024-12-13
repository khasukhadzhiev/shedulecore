using DTL.Dto.ScheduleDto;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using BL.ServiceInterface;

namespace CoreAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "employee")]
    [Route("api/[controller]")]
    [ApiController]
    public class LessonController : ControllerBase
    {
        private readonly ILessonService _lessonService;

        public LessonController(ILessonService lessonService)
        {
            _lessonService = lessonService;
        }

        [HttpGet]
        [Route("GetMainLessonList")]
        public async Task<IActionResult> GetMainLessonList(int studyClassId, int versionId)
        {
            var lessonList = await _lessonService.GetMainLessonListAsync(studyClassId, versionId);

            return Ok(lessonList);
        }

        [HttpGet]
        [Route("GetFlowLessonList")]
        public async Task<IActionResult> GetFlowList(int studyClassId, int versionId)
        {
            var lessonFlowDtoList = await _lessonService.GetFlowLessonListAsync(studyClassId, versionId);

            return Ok(lessonFlowDtoList);
        }

        [HttpGet]
        [Route("GetParallelLessonList")]
        public async Task<IActionResult> GetParallelList(int studyClassId, int versionId)
        {
            var lessonFlowDtoList = await _lessonService.GetParallelLessonListAsync(studyClassId, versionId);

            return Ok(lessonFlowDtoList);
        }

        [HttpGet]
        [Route("GetLessonTypeList")]
        public async Task<IActionResult> GetLessonTypeList()
        {
            var lessonTypeList = await _lessonService.GetLessonTypeListAsync();

            return Ok(lessonTypeList);
        }

        [HttpPost]
        [Route("AddLesson")]
        public async Task<IActionResult> AddLesson(LessonDto lessonDto, int versionId)
        {
            await _lessonService.AddMainLessonAsync(lessonDto, versionId);

            return Ok();
        }

        [HttpPost]
        [Route("AddFlow")]
        public async Task<IActionResult> AddFlow(LessonDto lessonDto, int versionId)
        {
            await _lessonService.AddFlowLessonAsync(lessonDto, versionId);

            return Ok();
        }

        [HttpPost]
        [Route("AddParallel")]
        public async Task<IActionResult> AddParallel(LessonDto lessonDto, int versionId)
        {
            await _lessonService.AddParallelLessonAsync(lessonDto, versionId);

            return Ok();
        }

        [HttpGet]
        [Route("RemoveLesson")]
        public async Task<IActionResult> RemoveLesson(int lessonId)
        {
            await _lessonService.RemoveLessonAsync(lessonId);

            return Ok();
        }

        [HttpGet]
        [Route("GetLessonList")]
        public async Task<IActionResult> GetLessonList(int studyClassId, int versionId)
        {
            var lessonList = await _lessonService.GetLessonListAsync(studyClassId, versionId);

            return Ok(lessonList);
        }

        [HttpPost]
        [Route("LessonSet")]
        public async Task<IActionResult> LessonSet(LessonDto lessonDto)
        {
            await _lessonService.LessonSetAsync(lessonDto);

            return Ok();
        }

        [HttpGet]
        [Route("ResetAllLessons")]
        public async Task<IActionResult> ResetAllLessons(int studyClassId)
        {
            await _lessonService.ResetAllLessonsAsync(studyClassId);

            return Ok();
        }

        [HttpPost]
        [Route("EditLessonData")]
        public async Task<IActionResult> EditLessonData(LessonDto lessonDto, int versionId)
        {
            await _lessonService.EditLessonDataAsync(lessonDto, versionId);

            return Ok();
        }

        [HttpGet]
        [Route("GetMainLessonFilterList")]
        public async Task<IActionResult> GetMainLessonFilterList(int studyClassId, int versionId, string filter)
        {
            var lessonList = await _lessonService.GetMainLessonFilterListAsync(studyClassId, versionId, filter);

            return Ok(lessonList);
        }

        [HttpGet]
        [Route("GetFlowLessonFilterList")]
        public async Task<IActionResult> GetFlowLessonFilterList(int studyClassId, int versionId, string filter)
        {
            var lessonList = await _lessonService.GetFlowLessonFilterListAsync(studyClassId, versionId, filter);

            return Ok(lessonList);
        }

        [HttpGet]
        [Route("GetParallelLessonFilterList")]
        public async Task<IActionResult> GetParallelLessonFilterList(int studyClassId, int versionId, string filter)
        {
            var lessonList = await _lessonService.GetParallelLessonFilterListAsync(studyClassId, versionId, filter);

            return Ok(lessonList);
        }
    }
}