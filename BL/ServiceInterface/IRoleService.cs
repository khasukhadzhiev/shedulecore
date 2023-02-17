using System.Collections.Generic;
using System.Threading.Tasks;
using DTL.Dto;

namespace BL.ServiceInterface
{
    public interface IRoleService
    {
        /// <summary>
        /// Получить список ролей в системе
        /// </summary>
        /// <returns>Список ролей в системе</returns>
        Task<List<RoleDto>> GetRoleListAsync();
    }
}
