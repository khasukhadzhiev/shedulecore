using DAL.Entities;

namespace DAL.Entities.Schedule
{
    public class Subdivision : BaseEntity
    {
        /// <summary>
        /// Наименование подразделения
        /// </summary>
        public string Name { get; set; }
    }
}
