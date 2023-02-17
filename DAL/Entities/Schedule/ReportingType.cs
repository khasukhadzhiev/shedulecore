using System.ComponentModel.DataAnnotations;
using DAL.Entities;

namespace DAL.Entities.Schedule
{
    public class ReportingType : BaseEntity
    {
        /// <summary>
        /// Наименование
        /// </summary>
        [Required]
        public string Name { get; set; }
    }
}
