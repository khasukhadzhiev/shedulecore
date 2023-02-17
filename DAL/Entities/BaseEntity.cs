using System.ComponentModel.DataAnnotations;

namespace DAL.Entities
{
    public class BaseEntity
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        [Required]
        [Key]
        public int Id { get; set; }
    }
}
