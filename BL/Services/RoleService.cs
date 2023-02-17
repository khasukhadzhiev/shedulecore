using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BL.ServiceInterface;
using DAL;
using DTL.Dto;
using DTL.Mapping;

namespace BL.Services
{
    public class RoleService : IRoleService
    {
        private readonly ScheduleHighSchoolDb _context;

        public RoleService(ScheduleHighSchoolDb context)
        {
            _context = context;
        }

        ///<inheritdoc/>
        public async Task<List<RoleDto>> GetRoleListAsync()
        {
            var roles = await _context.Roles
                .AsNoTracking()
                .OrderBy(r => r.Name)
                .Select(r => r.ToRoleDto())
                .ToListAsync();

            return roles;
        }
    }
}
