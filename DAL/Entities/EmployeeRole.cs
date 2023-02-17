using System.ComponentModel.DataAnnotations;

namespace DAL.Entities
{
    /// <summary>
    /// Роли сотрудника
    /// </summary>
    public class EmployeeRole : BaseEntity
    {
        /// <summary>
        /// Id пользователя
        /// </summary>
        [Required]
        public int EmployeeId { get; set; }

        /// <summary>
        /// Id роли
        /// </summary>
        [Required]
        public int RoleId { get; set; }

        /// <summary>
        /// Пользователь
        /// </summary>
        public Employee Employee { get; set; }

        /// <summary>
        /// Роль
        /// </summary>
        public Role Role { get; set; }
    }
}
