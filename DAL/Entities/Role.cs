using System.ComponentModel.DataAnnotations;

namespace DAL.Entities
{
    /// <summary>
    /// Роли
    /// </summary>
    public class Role : BaseEntity
    {
        /// <summary>
        /// Наименование роли
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Заголовок
        /// </summary>
        [Required]
        public string DisplayName { get; set; }
    }
}
