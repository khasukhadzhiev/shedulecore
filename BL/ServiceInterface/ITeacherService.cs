using DTL.Dto.ScheduleDto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BL.ServiceInterface
{
    public interface ITeacherService
    {
        /// <summary>
        /// Получить список преподавателей
        /// </summary>
        /// <returns>Список преподавателей</returns>
        Task<List<TeacherDto>> GetTeacherListAsync();

        /// <summary>
        /// Добавить преподавателя
        /// </summary>
        /// <param name="teacherDto">Преподаватель</param>
        /// <returns></returns>
        Task<string> AddTeacherAsync(TeacherDto teacherDto, int? versionId = null);

        /// <summary>
        /// Изменить преподавателя
        /// </summary>
        /// <param name="teacherDto">Преподаватель</param>
        /// <returns></returns>
        Task<string> EditTeacherAsync(TeacherDto teacherDto);

        /// <summary>
        /// Удалить преподавателя по ID
        /// </summary>
        /// <param name="id">Id преподавателя</param>
        /// <returns></returns>
        Task RemoveTeacherAsync(int id);
    }
}
