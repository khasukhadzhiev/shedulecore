namespace DTL.Dto.ScheduleDto
{
    /// <summary>
    /// Группа
    /// </summary>
    public class StudyClassDto : BaseDto
    {
        /// <summary>
        /// Количество учащихся
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
        public EducationFormDto EducationForm { get; set; }

        /// <summary>
        /// Смена обучения
        /// </summary>
        public ClassShiftDto ClassShift { get; set; }

        /// <summary>
        /// Подразделения
        /// </summary>
        public SubdivisionDto Subdivision { get; set; }
    }
}
