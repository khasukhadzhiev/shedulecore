using DAL.Entities;

namespace DAL.Entities.Schedule
{
    /// <summary>
    /// Дисциплины
    /// </summary>
    public class Subject : BaseEntity
    {
        /// <summary>
        /// Наименование дисциплины
        /// </summary>
        public string Name { get; set; }
    }
}
