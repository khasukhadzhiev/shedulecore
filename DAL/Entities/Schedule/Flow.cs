using DAL.Entities;

namespace DAL.Entities.Schedule
{
    /// <summary>
    /// Потоки (объедененные группы)
    /// </summary>
    public class Flow : BaseEntity
    {
        /// <summary>
        /// Наименование потока
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Id преподавателей
        /// </summary>
        public int[]? TeachersIds { get; set; }
    }
}
