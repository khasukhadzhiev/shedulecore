using DTL.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BL.ServiceInterface
{
    public interface IVersionService
    {
        /// <summary>
        /// Получить список версий расписания
        /// </summary>
        /// <returns>Список версий расписания</returns>
        Task<List<VersionDto>> GetVersionListAsync();

        /// <summary>
        /// Получить версию расписания с настройками
        /// </summary>
        /// <returns>Версия расписания с настройками</returns>
        Task<VersionDto> GetVersionAsync(int versionId);

        /// <summary>
        /// Получить активную версию расписания с настройками
        /// </summary>
        /// <returns>Активная версия расписания с настройками</returns>
        Task<VersionDto> GetActiveVersionAsync();

        /// <summary>
        /// Добавить новую версию расписания с настройками
        /// </summary>
        /// <param name="versionDto">Новая версия расписания с настройками</param>
        /// <returns></returns>
        Task<string> AddVersionAsync(VersionDto versionDto);

        /// <summary>
        /// Изменить версию расписания с настройками
        /// </summary>
        /// <param name="versionDto">Версия расписания с настройками</param>
        /// <returns></returns>
        Task<string> EditVersionAsync(VersionDto versionDto);

        /// <summary>
        /// Удалить версию расписания с настройками по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <returns></returns>
        Task RemoveVersionAsync(int id);
    }
}
