using System.Collections.Generic;

namespace DTL.Dto
{
    /// <summary>
    /// Аккаунт пользователя
    /// </summary>
    public class ClassroomSetResponseDto
    {
        /// <summary>
        /// Показать сообщения
        /// </summary>
        public bool ShowMessage { get; set; } = false;

        /// <summary>
        /// Имя пользователя
        /// </summary>
        public List<string> MessageList{ get; set; } = new List<string>();
    }
}
