using System.Collections.Generic;
using System.Threading.Tasks;

namespace BL.ServiceInterface
{
    public interface IMistakeService
    {
        /// <summary>
        /// Получить ошибоки расписания для группы
        /// </summary>
        /// <param name="studyClassId">>Id группы</param>
        /// <param name="versionId">Id версии</param>
        /// <returns></returns>
        Task<List<string>> GetMistakesByStudyClassAsync(int studyClassId, int versionId);

        /// <summary>
        /// Получить 
        /// </summary>
        /// <returns></returns>
        Task<List<string>> GetStudyClassNamesWithMistakesAsync();
    }
}
