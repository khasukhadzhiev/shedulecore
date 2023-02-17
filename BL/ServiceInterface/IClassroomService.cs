using DTL.Dto.ScheduleDto;
using System.Collections.Generic;
using System.Threading.Tasks;
using DTL.Dto;

namespace BL.ServiceInterface
{
    public interface IClassroomService
    {
        /// <summary>
        /// Получить список типов аудиторий
        /// </summary>
        /// <returns>Список типов аудиторий</returns>
        Task<List<ClassroomTypeDto>> GetClassroomTypeListAsync();

        /// <summary>
        /// Получить список аудиторий
        /// </summary>
        /// <returns>Список аудиторий</returns>
        Task<List<ClassroomDto>> GetClassroomListAsync();

        /// <summary>
        /// Добавить аудиторию
        /// </summary>
        /// <param name="classroomDto">Аудитория</param>
        /// <returns></returns>
        Task<string> AddClassroomAsync(ClassroomDto classroomDto);

        /// <summary>
        /// Изменить аудиторию
        /// </summary>
        /// <param name="classroomDto">Аудитория</param>
        /// <returns></returns>
        Task<string> EditClassroomAsync(ClassroomDto classroomDto);

        /// <summary>
        /// Удалить аудиторию по ID
        /// </summary>
        /// <param name="id">Id аудитории</param>
        /// <returns></returns>
        Task RemoveClassroomAsync(int id);

        /// <summary>
        /// Утсновить аудиторию для занятия
        /// </summary>
        /// <param name="lessonId">Id занятия</param>
        /// <param name="classroomId">Id аудитории</param>
        /// <returns></returns>
        Task SetClassroomByLessonAsync(int lessonId, int classroomId);

        /// <summary>
        /// Получить расписание по аудиториям
        /// </summary>
        /// <param name="selectedClassrooms"></param>
        /// <returns></returns>
        Task<List<QueryScheduleDto>> GetScheduleByClassroomsAsync(List<ClassroomDto> selectedClassroomListDto, int versionId);

        /// <summary>
        /// Получить предупреждения по аудитории
        /// </summary>
        /// <param name="lessonId">Id занятия</param>
        /// <param name="classroomId">Id аудитории</param>
        /// <returns></returns>
        Task<ClassroomSetResponseDto> GetWarningsByClassroomAsync(int lessonId);
    }
}
