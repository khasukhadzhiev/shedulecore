using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using BL.ServiceInterface;
using DTL.Dto;

namespace CoreAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        [Route("GetEmployeeList")]
        public async Task<IActionResult> GetEmployeeList()
        {
            var employeeList = await _employeeService.GetEmployeeListAsync();

            return Ok(employeeList);
        }

        [HttpPost]
        [Route("AddEmployee")]
        public async Task<IActionResult> AddEmployee(EmployeeDto employeeDto)
        {
            await _employeeService.AddEmployeeAsync(employeeDto);

            return Ok();
        }

        [HttpPost]
        [Route("EditEmployee")]
        public async Task<IActionResult> EditEmployee(EmployeeDto employeeDto)
        {
            await _employeeService.EditEmployeeAsync(employeeDto);

            return Ok();
        }

        [HttpGet]
        [Route("DeleteEmployee")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            await _employeeService.DeleteEmployeeAsync(id);

            return Ok();
        }
    }
}