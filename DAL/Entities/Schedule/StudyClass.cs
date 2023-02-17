using DAL.Entities;

namespace DAL.Entities.Schedule
{
    /// <summary>
    /// Группы обучения
    /// </summary>
    public class StudyClass : BaseEntity
    {
        /// <summary>
        /// Наименование группы
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Количество учащихся в группе
        /// </summary>
        public int StudentsCount { get; set; }

        /// <summary>
        /// Id формы обучения
        /// </summary>
        public int EducationFormId { get; set; }

        /// <summary>
        /// Id смены обучения
        /// </summary>
        public int ClassShiftId { get; set; }

        /// <summary>
        /// Id подразделения
        /// </summary>
        public int SubdivisionId { get; set; }

        /// <summary>
        /// Форма обучения
        /// </summary>
        public EducationForm EducationForm { get; set; }

        /// <summary>
        /// Смена обучения
        /// </summary>
        public ClassShift ClassShift { get; set; }

        /// <summary>
        /// Подразделения
        /// </summary>
        public Subdivision Subdivision { get; set; }
    }
}
