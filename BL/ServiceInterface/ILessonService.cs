using DTL.Dto.ScheduleDto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BL.ServiceInterface
{
    public interface ILessonService
    {
        /// <summary>
        /// Получить все занятия группы
        /// </summary>
        /// <param name="studyClassId">Id группы</param>
        /// <returns>Список занятий</returns>
        Task<List<LessonDto>> GetMainLessonListAsync(int studyClassId, int versionId);

        /// <summary>
        /// Добавить занятие
        /// </summary>
        /// <param name="lesson">Занятие</param>
        /// <returns></returns>
        Task AddMainLessonAsync(LessonDto lesson, int versionId);

        /// <summary>
        /// Получить список поточных занятий
        /// </summary>
        /// <returns>Список поточных занятий</returns>
        Task<List<LessonDto>> GetFlowLessonListAsync(int studyClassId, int versionId);

        /// <summary>
        /// Добавить поточное занятие
        /// </summary>
        /// <param name="lesson">Занятие</param>
        /// <returns></returns>
        Task AddFlowLessonAsync(LessonDto lessonDto, int versionId);

        /// <summary>
        /// Получить список паралелей
        /// </summary>
        /// <returns>Список паралелей</returns>
        Task<List<LessonDto>> GetParallelLessonListAsync(int studyClassId, int versionId);

        /// <summary>
        /// Добавить паралель
        /// </summary>
        /// <param name="lesson">Занятие</param>
        /// <returns></returns>
        Task AddParallelLessonAsync(LessonDto lesson, int versionId);

        /// <summary>
        /// Удалить занятие
        /// </summary>
        /// <param name="lessonId">Id занятия</param>
        /// <returns></returns>
        Task RemoveLessonAsync(int lessonId);

        /// <summary>
        /// Получить виды занятий
        /// </summary>
        /// <returns>Список видов занятий</returns>
        Task<List<LessonTypeDto>> GetLessonTypeListAsync();

        /// <summary>
        /// Получить все занятия группы
        /// </summary>
        /// <param name="studyClassId">Id группы</param>
        /// <param name="versionId">Id версии</param>
        /// <returns>Список занятий</returns>
        Task<List<LessonDto>> GetAllLessonListByStudyClassAsync(int studyClassId, int versionId);

        /// <summary>
        /// Установить значения распределения занятия. (Индекс строки и индекс столбца в таблице)
        /// </summary>
        /// <returns></returns>
        Task LessonSetAsync(LessonDto lessonDto);

        /// <summary>
        /// Сбросить расписание для группы
        /// </summary>
        /// <param name="studyClassId">Id группы</param>
        /// <returns></returns>
        Task ResetAllLessonsAsync(int studyClassId);

        /// <summary>
        /// Обновить дисциплину и преподавателя занятия
        /// </summary>
        /// <param name="lesson">Занятие</param>
        /// <param name="versionId">Id версии расписания</param>
        /// <returns></returns>
        Task EditLessonDataAsync(LessonDto lesson, int versionId);

        #region Filtering lesson

        /// <summary>
        /// Получить занятия по фильтру
        /// </summary>
        /// <param name="studyClassId">Id группы</param>
        /// <param name="versionId">Id версии расписания</param>
        /// <param name="filter">Фильтр</param>
        /// <returns></returns>
        Task<List<LessonDto>> GetMainLessonFilterListAsync(int studyClassId, int versionId, string filter);

        /// <summary>
        /// Получить потоки по фильтру
        /// </summary>
        /// <param name="studyClassId">Id группы</param>
        /// <param name="versionId">Id версии расписания</param>
        /// <param name="filter">Фильтр</param>
        /// <returns></returns>
        Task<List<LessonDto>> GetFlowLessonFilterListAsync(int studyClassId, int versionId, string filter);

        /// <summary>
        /// Получить паралели по фильтру
        /// </summary>
        /// <param name="studyClassId">Id группы</param>
        /// <param name="versionId">Id версии расписания</param>
        /// <param name="filter">Фильтр</param>
        /// <returns></returns>
        Task<List<LessonDto>> GetParallelLessonFilterListAsync(int studyClassId, int versionId, string filter);
        #endregion
    }
}
