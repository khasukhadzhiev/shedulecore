using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Models
{
    /// <summary>
    /// Модель списка выводимых накладок
    /// </summary>
    public class MistakeListModel
    {
        /// <summary>
        /// День
        /// </summary>
        public string Day { get; set; }

        /// <summary>
        /// Пара
        /// </summary>
        public string Para { get; set; }

        /// <summary>
        /// Вид накладки
        /// </summary>
        public string MistakeType { get; set; }

        /// <summary>
        /// Объект накладки
        /// </summary>
        public string MistakeObject { get; set; }

        /// <summary>
        /// Группа накладки
        /// </summary>
        public string StudyClass { get; set; }
    }
}
