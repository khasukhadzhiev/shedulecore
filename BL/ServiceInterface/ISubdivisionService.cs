using DTL.Dto.ScheduleDto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BL.ServiceInterface
{
    public interface ISubdivisionService
    {
        /// <summary>
        /// Получить список подразделений
        /// </summary>
        /// <returns>Список подразделений</returns>
        Task<List<SubdivisionDto>> GetSubdivisionListAsync();

        /// <summary>
        /// Получить список подразделений связанных с сотрудником
        /// </summary>
        /// <returns>Список подразделений связанных с сотрудником</returns>
        Task<List<SubdivisionDto>> GetCurrentEmployeeSubdivisionListAsync();

        /// <summary>
        /// Добавить подразделение
        /// </summary>
        /// <param name="subdivisionDto">Новое подразделение</param>
        /// <returns></returns>
        Task<string> AddSubdivisionAsync(SubdivisionDto subdivisionDto);

        /// <summary>
        /// Изменить подразделение
        /// </summary>
        /// <param name="subdivisionDto">Новые данные подразделения</param>
        /// <returns></returns>
        Task<string> EditSubdivisionAsync(SubdivisionDto subdivisionDto);

        /// <summary>
        /// Удалить подразделение
        /// </summary>
        /// <param name="id">Id подразделения</param>
        /// <returns></returns>
        Task RemoveSubdivisionAsync(int id);
    }
}
