using System.Collections.Generic;
using System.Threading.Tasks;
using DTL.Dto;

namespace BL.ServiceInterface
{
    public interface IEmployeeService
    {
        /// <summary>
        /// Получить список пользователей
        /// </summary>
        /// <returns>Список пользователей</returns>
        Task<List<EmployeeDto>> GetEmployeeListAsync();

        /// <summary>
        /// Изменить данные сотруника
        /// </summary>
        /// <param name="employeeDto">Новые данные сотруника</param>
        /// <returns></returns>
        Task EditEmployeeAsync(EmployeeDto employeeDto);

        /// <summary>
        /// Добавить сотрудника
        /// </summary>
        /// <param name="employeeDto">Сотрудник</param>
        /// <returns></returns>
        Task AddEmployeeAsync(EmployeeDto employeeDto);

        /// <summary>
        /// Удалить пользователя по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <returns></returns>
        Task DeleteEmployeeAsync(int id);
    }
}
