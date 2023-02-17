using DTL.Dto.ScheduleDto;
using System.Collections.Generic;

namespace DTL.Dto
{
    /// <summary>
    /// Сотрудник
    /// </summary>
    public class EmployeeDto
    {
        public int? Id { get; set; }

        /// <summary>
        /// Фамилия
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Имя
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Отчество
        /// </summary>
        public string MiddleName { get; set; }

        /// <summary>
        /// Действующий пользователь
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Аккаунт пользователя
        /// </summary>
        public AccountDto Account { get; set; }

        /// <summary>
        /// Список ролей пользователя
        /// </summary>
        public List<RoleDto> Roles { get; set; }

        /// <summary>
        /// Список связанных подразделений
        /// </summary>
        public List<SubdivisionDto> SubdivisionList { get; set; }
    }
}
