using System.ComponentModel.DataAnnotations;
using DAL.Entities;

namespace DAL.Entities.Schedule
{
    /// <summary>
    /// Форма обучения
    /// </summary>
    public class EducationForm : BaseEntity
    {
        /// <summary>
        /// Наименование формы обучения
        /// </summary>
        [Required]
        public string Name { get; set; }
    }
}
