using DTL.Dto.ScheduleDto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BL.ServiceInterface
{
    public interface ISubjectService
    {
        /// <summary>
        /// Получить список дисциплин
        /// </summary>
        /// <returns>Список дисциплин</returns>
        Task<List<SubjectDto>> GetSubjectListAsync();

        /// <summary>
        /// Добавить дисциплину
        /// </summary>
        /// <param name="subjectDto">Дисциплина</param>
        /// <returns></returns>
        Task<string> AddSubjectAsync(SubjectDto subjectDto);

        /// <summary>
        /// Изменить дисциплину
        /// </summary>
        /// <param name="subjectDto">Дисциплина</param>
        /// <returns></returns>
        Task<string> EditSubjectAsync(SubjectDto subjectDto);

        /// <summary>
        /// Удалить дисциплину по ID
        /// </summary>
        /// <param name="id">Id дисциплины</param>
        /// <returns></returns>
        Task RemoveSubjectAsync(int id);
    }
}
