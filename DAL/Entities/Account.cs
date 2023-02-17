using System.ComponentModel.DataAnnotations;

namespace DAL.Entities
{
    /// <summary>
    /// Аккаунт пользователя
    /// </summary>
    public class Account : BaseEntity
    {
        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// Пароль
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Действующий аккаунт
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Id пользователя
        /// </summary>
        [Required]
        public int EmployeeId { get; set; }

        /// <summary>
        /// Пользователь
        /// </summary>
        public Employee Employee { get; set; }
    }
}
