using DTL.Dto.ScheduleDto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BL.ServiceInterface
{
    public interface IBuildingService
    {
        /// <summary>
        /// Получить список корпусов
        /// </summary>
        /// <returns>Список аудиторий</returns>
        Task<List<BuildingDto>> GetBuildingListAsync();

        /// <summary>
        /// Добавить корпус
        /// </summary>
        /// <param name="buildingDto">Корпус</param>
        /// <returns></returns>
        Task<string> AddBuildingAsync(BuildingDto buildingDto);

        /// <summary>
        /// Изменить корпус
        /// </summary>
        /// <param name="buildingDto">Корпус</param>
        /// <returns></returns>
        Task<string> EditBuildingAsync(BuildingDto buildingDto);

        /// <summary>
        /// Удалить корпус по ID
        /// </summary>
        /// <param name="id">Id аудитории</param>
        /// <returns></returns>
        Task RemoveBuildingAsync(int id);
    }
}
