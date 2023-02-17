using System.Collections.Generic;

namespace DTL.Dto.ScheduleDto
{
    public class QueryScheduleDto
    {
        /// <summary>
        /// Флаг обозначающий, что расписание для группы
        /// </summary>
        public bool IsStudyClass { get; set; }

        /// <summary>
        /// Занятия
        /// </summary>
        public List<LessonDto> LessonList { get; set; }

        /// <summary>
        /// Запрос
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Запрос
        /// </summary>
        public string Name { get; set; }

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
