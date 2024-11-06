using DTL.Dto.ScheduleDto;

namespace BL.ServiceInterface
{
    public interface IClassroomTypeService
    {
        /// <summary>
        /// Получить список типов аудиторий
        /// </summary>
        /// <returns>Список типов аудиторий</returns>
        Task<List<ClassroomTypeDto>> GetClassroomTypeListAsync();

        /// <summary>
        /// Добавить тип аудиторию
        /// </summary>
        /// <param name="classroomDto">Аудитория</param>
        /// <returns></returns>
        Task<string> AddClassroomTypeAsync(ClassroomTypeDto classroomTypeDto);

        /// <summary>
        /// Изменить тип аудитории
        /// </summary>
        /// <param name="classroomDto">Аудитория</param>
        /// <returns></returns>
        Task<string> EditClassroomTypeAsync(ClassroomTypeDto classroomTypeDto);

        /// <summary>
        /// Удалить тип аудиторию по ID
        /// </summary>
        /// <param name="id">Id аудитории</param>
        /// <returns></returns>
        Task RemoveClassroomTypeAsync(int id);
    }
}
