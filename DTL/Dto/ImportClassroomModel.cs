namespace DTL.Dto
{
    public class ImportClassroomModel
    {
        /// <summary>
        /// Наименование корпуса
        /// </summary>
        public string BuildingName { get; set; }

        /// <summary>
        /// Тип аудитории
        /// </summary>
        public string ClassroomType { get; set; }

        /// <summary>
        /// Наименование аудитории
        /// </summary>
        public string ClassroomName { get; set; }

        /// <summary>
        /// Число посадочных мест
        /// </summary>
        public int SeatsCount { get; set; }
    }
}
