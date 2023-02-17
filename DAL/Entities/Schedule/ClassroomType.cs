using DAL.Entities;

namespace DAL.Entities.Schedule
{
    /// <summary>
    /// Тип аудитории
    /// </summary>
    public class ClassroomType : BaseEntity
    {
        /// <summary>
        /// Наименование типа аудитории
        /// </summary>
        public string Name { get; set; }
    }
}
