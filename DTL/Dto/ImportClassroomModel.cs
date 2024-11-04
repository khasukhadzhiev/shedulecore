namespace DTL.Dto
{
    public class ImportClassroomModel
    {
        /// <summary>
        /// Наименование аудитории
        /// </summary>
        public string ClassroomName { get; set; }

        /// <summary>
        /// Тип аудитории
        /// </summary>
        public string ClassroomType { get; set; }

        /// <summary>
        /// Число посадочных мест
        /// </summary>
        public int SeatsCount { get; set; }

        /// <summary>
        /// Наименование корпуса
        /// </summary>
        public int BuildingName { get; set; }
    }
}
