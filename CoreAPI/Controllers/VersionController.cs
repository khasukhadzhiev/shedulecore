using DTL.Dto;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using BL.ServiceInterface;

namespace CoreAPI.Controllers
{

    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class VersionController : ControllerBase
    {
        private readonly IVersionService _versionService;

        public VersionController(IVersionService versionService)
        {
            _versionService = versionService;
        }

        [HttpGet]
        [Route("GetVersionList")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin, employee")]
        public async Task<IActionResult> GetVersionList()
        {
            var versionList = await _versionService.GetVersionListAsync();

            return Ok(versionList);
        }

        [HttpGet]
        [Route("GetActiveVersion")]
        [AllowAnonymous]
        public async Task<IActionResult> GetActiveVersion()
        {
            var versionDto = await _versionService.GetActiveVersionAsync();

            return Ok(versionDto);
        }

        [HttpGet]
        [Route("GetVersion")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin, employee")]
        public async Task<IActionResult> GetVersion(int versionId)
        {
            var versionDto = await _versionService.GetVersionAsync(versionId);

            return Ok(versionDto);
        }

        [HttpPost]
        [Route("AddVersion")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> AddVersion(VersionDto versionDto)
        {
            var result = await _versionService.AddVersionAsync(versionDto);

            return Ok(result);
        }

        [HttpPost]
        [Route("EditVersion")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> EditVersion(VersionDto versionDto)
        {
            var result = await _versionService.EditVersionAsync(versionDto);

            return Ok(result);
        }

        [HttpGet]
        [Route("RemoveVersion")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<IActionResult> RemoveVersion(int id)
        {
            await _versionService.RemoveVersionAsync(id);

            return Ok();
        }
    }
}