namespace DTL.Dto
{
    public class ImportDataModel
    {
        /// <summary>
        /// Подразделение
        /// </summary>
        public string Subdivision { get; set; }

        /// <summary>
        /// Группа
        /// </summary>
        public string StudyClass { get; set; }

        /// <summary>
        /// Количество обучающихся
        /// </summary>
        public int StudentsCount { get; set; }

        /// <summary>
        /// Преподаватель
        /// </summary>
        public string Teacher { get; set; }

        /// <summary>
        /// Дисциплина
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Вид занятия
        /// </summary>
        public string LessonType { get; set; }

        /// <summary>
        /// Занятие подгруппы
        /// </summary>
        public bool IsSubclassLesson { get; set; }

        /// <summary>
        /// Занятие по одной неделе
        /// </summary>
        public bool IsSubweekLesson { get; set; }

        /// <summary>
        /// Параллель
        /// </summary>
        public bool IsParallel { get; set; }

        /// <summary>
        /// Поток
        /// </summary>
        public bool IsFlow { get; set; }

        /// <summary>
        /// Группы потока
        /// </summary>
        public string FlowStudyClassNames { get; set; }

        /// <summary>
        /// Смена
        /// </summary>
        public string ClassShift { get; set; }

        /// <summary>
        /// Форма обучения
        /// </summary>
        public string EducationForm { get; set; }

        /// <summary>
        /// Форма обучения
        /// </summary>
        public string StudyClassReportingName { get; set; }
    }
}
