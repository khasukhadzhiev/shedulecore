using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DAL.Entities
{
    /// <summary>
    /// Сотрудник
    /// </summary>
    public class Employee : BaseEntity
    {
        /// <summary>
        /// Фамилия
        /// </summary>
        [Required]
        public string FirstName { get; set; }

        /// <summary>
        /// Имя
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Отчество
        /// </summary>
        public string MiddleName { get; set; }

        /// <summary>
        /// Аккаунт пользователя
        /// </summary>
        public Account Account { get; set; }

        /// <summary>
        /// Список ролей пользователя
        /// </summary>
        public List<EmployeeRole> EmployeeRoles { get; set; }
    }
}
