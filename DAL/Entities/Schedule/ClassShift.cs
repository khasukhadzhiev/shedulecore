using System.ComponentModel.DataAnnotations;
using DAL.Entities;

namespace DAL.Entities.Schedule
{
    /// <summary>
    /// Смена обучения
    /// </summary>
    public class ClassShift : BaseEntity
    {
        /// <summary>
        /// Наименование смены обучения
        /// </summary>
        [Required]
        public string Name { get; set; }
    }
}
