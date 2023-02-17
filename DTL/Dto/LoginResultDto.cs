using System;

namespace DTL.Dto
{
    /// <summary>
    /// Модель результата авторизации
    /// </summary>
    public class LoginResultDto
    {
        /// <summary>
        /// Токен авторизации
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Роли пользователя
        /// </summary>
        public string[] EmployeeRoles { get; set; }

        /// <summary>
        /// ФИО пользователя
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Пользователь авторизован
        /// </summary>
        public bool IsAutentificated { get; set; }

        /// <summary>
        /// Продолжительность авторизации
        /// </summary>
        public DateTime Expiration { get; set; }

        /// <summary>
        /// Действующий пользователь
        /// </summary>
        public bool IsValid { get; set; }
    }
}
