using DTL.Dto;
using Infrastructure;
using System.Threading.Tasks;

namespace BL.ServiceInterface
{
    public interface IGeneticAlgorithmService
    {
        /// <summary>
        /// Начать генерацию расписания
        /// </summary>
        /// <param name="geneticAlgorithmDataDto">Данные для формирования расписания</param>
        /// <returns></returns>
        Task GenerateScheduleAsync(GeneticAlgorithmDataDto geneticAlgorithmDataDto);

        /// <summary>
        /// Получить прогресс генерации расписания
        /// </summary>
        /// <returns></returns>
        Task<ScheduleGeneratedProgress> GetScheduleGenerateProgressAsync();

        /// <summary>
        /// Остановить генрацию расписания
        /// </summary>
        /// <returns></returns>
        Task StopScheduleGenerateAsync();

        /// <summary>
        /// Сохранить расписание с накладками
        /// </summary>
        /// <returns></returns>
        Task SaveScheduleWithMistakesAsync();
    }
}
