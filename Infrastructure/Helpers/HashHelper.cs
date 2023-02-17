using System;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Helpers
{
    /// <summary>
    /// HashHelper
    /// </summary>
    public class HashHelper
	{
        /// <summary>
        /// Формирует и возращает Hash для переданного пароля
        /// </summary>
        /// <param name="password">Пароль</param>
        /// <returns>Hash строка</returns>
        public static string HashPassword(string password)
        {
            byte[] data = Encoding.Default.GetBytes(password);
            SHA1 sha = SHA1.Create();
            byte[] result = sha.ComputeHash(data);
            password = Convert.ToBase64String(result);
            return password;
        }
    }
}

