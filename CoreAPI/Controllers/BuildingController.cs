using BL.ServiceInterface;
using DTL.Dto.ScheduleDto;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CoreAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "employee")]
    [ApiController]
    public class BuildingController : ControllerBase
    {
        private readonly IBuildingService _buildingService;

        public BuildingController(IBuildingService buildingService)
        {
            _buildingService = buildingService;
        }

        [HttpGet]
        [Route("GetBuildingList")]
        public async Task<IActionResult> GetBuildingList()
        {
            var classrooms = await _buildingService.GetBuildingListAsync();

            return Ok(classrooms);
        }

        [HttpPost]
        [Route("AddBuilding")]
        public async Task<IActionResult> AddBuilding(BuildingDto buildingDto)
        {
            var result = await _buildingService.AddBuildingAsync(buildingDto);

            return Ok(result);
        }

        [HttpPost]
        [Route("EditBuilding")]
        public async Task<IActionResult> EditBuilding(BuildingDto buildingDto)
        {
            var result = await _buildingService.EditBuildingAsync(buildingDto);

            return Ok(result);
        }

        [HttpGet]
        [Route("RemoveBuilding")]
        public async Task<IActionResult> RemoveBuilding(int id)
        {
            await _buildingService.RemoveBuildingAsync(id);

            return Ok();
        }
    }
}
