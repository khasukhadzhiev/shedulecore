using DAL.Entities;

namespace DAL.Entities
{
    /// <summary>
    /// Настройки приложения
    /// </summary>
    public class Version : BaseEntity
    {
        /// <summary>
        /// Наименование версии расписания
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Активная версия
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Максимальное колчество занятий в день
        /// </summary>
        public int MaxLesson { get; set; }

        /// <summary>
        /// Использовать воскресенье в расписании
        /// </summary>
        public bool UseSunday { get; set; }

        /// <summary>
        /// Использовать деление по неделям
        /// </summary>
        public bool UseSubWeek { get; set; }

        /// <summary>
        /// Использовать деление на подгруппы
        /// </summary>
        public bool UseSubClass { get; set; }

        /// <summary>
        /// Показывать форму обучения
        /// </summary>
        public bool ShowEducationForm { get; set; }

        /// <summary>
        /// Показывать смену обучения
        /// </summary>
        public bool ShowClassShift { get; set; }

        /// <summary>
        /// Id выводимых отчетностей
        /// </summary>
        public string[] ShowReportingIds { get; set; }
    }
}
