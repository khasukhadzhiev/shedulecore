using DTL.Dto;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using BL.ServiceInterface;

namespace CoreAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class TimetableController : ControllerBase
    {
        private readonly IMistakeService _mistakeService;
        private readonly IGeneticAlgorithmService _geneticAlgorithmService;

        public TimetableController(IMistakeService mistakeService, IGeneticAlgorithmService geneticAlgorithmService)
        {
            _mistakeService = mistakeService;
            _geneticAlgorithmService = geneticAlgorithmService;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "employee")]
        [Route("GetMistakesByStudyClass")]
        public async Task<IActionResult> GetMistakesByStudyClass(int studyClassId, int versionId)
        {
            var mistakes = await _mistakeService.GetMistakesByStudyClassAsync(studyClassId, versionId);

            return Ok(mistakes);
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "employee")]
        [Route("GetStudyClassNamesWithMistakes")]
        public async Task<IActionResult> GetStudyClassNamesWithMistakes()
        {
            var mistakes = await _mistakeService.GetStudyClassNamesWithMistakesAsync();

            return Ok(mistakes);
        }


        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        [Route("GenerateTimetable")]
        public async Task<IActionResult> GenerateTimetable(GeneticAlgorithmDataDto geneticAlgorithmDataDto)
        {
            await _geneticAlgorithmService.GenerateScheduleAsync(geneticAlgorithmDataDto);

            return Ok();
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin, employee")]
        [Route("GetTimetableGenerateProgress")]
        public async Task<IActionResult> GetTimetableGenerateProgress()
        {
            var result = await _geneticAlgorithmService.GetScheduleGenerateProgressAsync();

            return Ok(result);
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        [Route("StopTimetableGenerate")]
        public async Task<IActionResult> StopTimetableGenerate()
        {
            await _geneticAlgorithmService.StopScheduleGenerateAsync();

            return Ok();
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        [Route("SaveTimetableWithMistakes")]
        public async Task<IActionResult> SaveTimetableWithMistakes()
        {
            await _geneticAlgorithmService.SaveScheduleWithMistakesAsync();

            return Ok();
        }

    }
}