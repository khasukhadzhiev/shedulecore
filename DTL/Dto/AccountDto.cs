namespace DTL.Dto
{
    /// <summary>
    /// Аккаунт пользователя
    /// </summary>
    public class AccountDto
    {
        /// <summary>
        /// Id аккаунта
        /// </summary>
        public int Id { get; set; }

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
    }
}
