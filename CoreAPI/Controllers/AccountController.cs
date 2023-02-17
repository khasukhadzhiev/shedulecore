using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using BL.ServiceInterface;
using DTL.Dto;

namespace CoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }


        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> LogIn(AccountDto accountDto)
        {
            var result = await _accountService.LogInAsync(accountDto);

            if (result != null && result.IsAutentificated)
            {
                return Ok(result);
            }
            else if (result != null && !result.IsValid)
            {
                return BadRequest("Ваш аккаунт заблокирован! Обратитесь к администратору.");
            }
            else
            {
                return BadRequest("Неверный логин или пароль.");
            }
        }
    }
}