using DTL.Dto.ScheduleDto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BL.ServiceInterface
{
    public interface IStudyClassService
    {
        /// <summary>
        /// Получить список групп, со всеми данными
        /// </summary>
        /// <returns>Список групп</returns>
        Task<List<StudyClassDto>> GetStudyClassListAsync();

        /// <summary>
        /// Получить список групп для подразделения
        /// </summary>
        /// <param name="subdivisionId">Id подразделения</param>
        /// <returns>Список групп</returns>
        Task<List<List<StudyClassDto>>> GetStudyClassListBySubdivisionAsync(int subdivisionId);

        /// <summary>
        /// Добавить группу
        /// </summary>
        /// <param name="studyClassDto">Группа</param>
        /// <returns></returns>
        Task<string> AddStudyClassAsync(StudyClassDto studyClassDto);

        /// <summary>
        /// Изменить данные группы
        /// </summary>
        /// <param name="studyClassDto">Новые данные группы</param>
        /// <returns></returns>
        Task<string> EditStudyClassAsync(StudyClassDto studyClassDto);

        /// <summary>
        /// Удалить группу по ID
        /// </summary>
        /// <param name="id">Id группы</param>
        /// <returns></returns>
        Task RemoveStudyClassAsync(int id);

        /// <summary>
        /// Получить список форм обучения
        /// </summary>
        /// <returns>Список форм обучения</returns>
        Task<List<EducationFormDto>> GetEducationFormListAsync();

        /// <summary>
        /// Получить список смен обучения
        /// </summary>
        /// <returns>Список смен обучения</returns>
        Task<List<ClassShiftDto>> GetClassShiftListAsync();


    }
}
