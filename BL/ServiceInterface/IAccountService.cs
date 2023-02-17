using System.Threading.Tasks;
using DTL.Dto;

namespace BL.ServiceInterface
{
    public interface IAccountService
    {
        /// <summary>
        /// Вход в систему
        /// </summary>
        /// <param name="logindata">Данные входа</param>
        /// <returns>Результат авторизации</returns>
        Task<LoginResultDto> LogInAsync(AccountDto accountDto);
    }
}
