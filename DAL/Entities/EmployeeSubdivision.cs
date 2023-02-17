using DAL.Entities.Schedule;
using System.ComponentModel.DataAnnotations;
using DAL.Entities;

namespace DAL.Entities
{
    /// <summary>
    /// Подразделения прикрепленные к сотруднику
    /// </summary>
    public class EmployeeSubdivision : BaseEntity
    {
        /// <summary>
        /// Id пользователя
        /// </summary>
        [Required]
        public int EmployeeId { get; set; }

        /// <summary>
        /// Id подразделения
        /// </summary>
        [Required]
        public int SubdivisionId { get; set; }

        /// <summary>
        /// Сотрудник
        /// </summary>
        public Employee Employee { get; set; }

        /// <summary>
        /// Подразделение
        /// </summary>
        public Subdivision Subdivision { get; set; }
    }
}
