using System;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Infrastructure.Models
{
    /// <summary>
    /// AuthorizeOptions
    /// </summary>
    public class AuthorizeOptions
	{
        /// <summary>
        /// Издатель токена
        /// </summary>
        public const string ISSUER = "ScheduleCoreINC";

        /// <summary>
        /// Потребитель токена
        /// </summary>
        public const string AUDIENCE = "ScheduleCoreINCClient";

        /// <summary>
        /// Ключ для шифрации
        /// </summary>
        const string KEY = "2042E3AD-B1D4-4FC2-A5E1-F55D937089E9";

        /// <summary>
        /// Время жизни токена (4 часа)
        /// </summary>
        public const int LIFETIME = 4;

        /// <summary>
        /// Получить ключ
        /// </summary>
        /// <returns>Строка-ключ</returns>
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}

