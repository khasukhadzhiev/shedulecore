using DAL.Entities;
using DTL.Dto.ScheduleDto;
using DTL.Mapping;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BL.ServiceInterface;
using DAL;
using DTL.Dto;
using Infrastructure.Helpers;

namespace BL.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly ScheduleHighSchoolDb _context;

        public EmployeeService(ScheduleHighSchoolDb context)
        {
            _context = context;
        }

        ///<inheritdoc/>
        public async Task<List<EmployeeDto>> GetEmployeeListAsync()
        {
            var employeeList = await _context.Employees
                .AsNoTracking()
                .Include(u => u.Account)
                .Where(u => u.Id != 1)
                .OrderBy(u => u.FirstName).ThenBy(u => u.Name).ThenBy(u => u.MiddleName)
                .Select(res => res.ToEmployeeDto())
                .ToListAsync();

            foreach (var employee in employeeList)
            {
                employee.Roles = _context.EmployeeRoles
                    .Include(r => r.Role)
                    .Where(r => r.EmployeeId == employee.Id)
                    .Select(r => r.Role.ToRoleDto())
                    .ToList();

                employee.SubdivisionList = _context.EmployeeSubdivisions
                    .Include(r => r.Subdivision)
                    .Where(r => r.EmployeeId == employee.Id)
                    .Select(r => r.Subdivision.ToDto<SubdivisionDto>())
                    .ToList();
            }

            return employeeList;
        }

        ///<inheritdoc/>
        public async Task AddEmployeeAsync(EmployeeDto employeeDto)
        {
            if (string.IsNullOrEmpty(employeeDto.FirstName.Trim()) || string.IsNullOrEmpty(employeeDto.Name.Trim())
                || string.IsNullOrEmpty(employeeDto.Account.Login.Trim()) || string.IsNullOrEmpty(employeeDto.Account.Password.Trim()))
            {
                throw new Exception("ФИО, Логин и Пароль нового сотрудника должны быть заполнены.");
            }

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var employee = employeeDto.ToEmployee();

                    _context.Employees.Add(employee);

                    await _context.SaveChangesAsync();

                    var account = employeeDto.ToAccount(employee);

                    _context.Accounts.Add(account);

                    var newEmployeeRoles = employeeDto.Roles.Select(ur => new EmployeeRole
                    {
                        RoleId = ur.Id,
                        EmployeeId = employee.Id
                    });

                    _context.EmployeeRoles.AddRange(newEmployeeRoles);

                    var newSubdivisionList = employeeDto.SubdivisionList.Select(s => new EmployeeSubdivision
                    {
                        SubdivisionId = s.Id,
                        EmployeeId = employee.Id
                    });

                    _context.EmployeeSubdivisions.AddRange(newSubdivisionList);

                    await _context.SaveChangesAsync();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    throw new Exception(ex.Message);
                }
            }
        }

        ///<inheritdoc/>
        public async Task EditEmployeeAsync(EmployeeDto employeeDto)
        {
            if (string.IsNullOrEmpty(employeeDto.FirstName.Trim()) || string.IsNullOrEmpty(employeeDto.Name.Trim())
                || string.IsNullOrEmpty(employeeDto.Account.Login.Trim()) || string.IsNullOrEmpty(employeeDto.Account.Password.Trim()))
            {
                throw new Exception("ФИО, Логин и Пароль нового сотрудника должны быть заполнены.");
            }

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var employee = await _context.Employees.Include(u => u.Account).FirstOrDefaultAsync(u => u.Id == employeeDto.Id);

                    var oldPassword = employee.Account.Password;

                    var newEmployeeData = employeeDto.ToEmployee();

                    employee.FirstName = newEmployeeData.FirstName;
                    employee.Name = newEmployeeData.Name;
                    employee.MiddleName = newEmployeeData.MiddleName;
                    employee.Account = employeeDto.ToAccount(employee);

                    if (oldPassword != employeeDto.Account.Password)
                    {
                        employee.Account.Password = HashHelper.HashPassword(employeeDto.Account.Password);
                    }
                    else
                    {
                        employee.Account.Password = oldPassword;
                    }

                    var employeeOldRoles = _context.EmployeeRoles.Where(ur => ur.EmployeeId == employeeDto.Id);

                    _context.EmployeeRoles.RemoveRange(employeeOldRoles);

                    var newEmployeeRoles = employeeDto.Roles.Select(ur => new EmployeeRole
                    {
                        RoleId = ur.Id,
                        EmployeeId = employeeDto.Id.Value
                    });

                    _context.EmployeeRoles.AddRange(newEmployeeRoles);

                    var oldSubdivisionList = _context.EmployeeSubdivisions.Where(ur => ur.EmployeeId == employeeDto.Id);

                    _context.EmployeeSubdivisions.RemoveRange(oldSubdivisionList);

                    var newSubdivisionList = employeeDto.SubdivisionList.Select(s => new EmployeeSubdivision
                    {
                        SubdivisionId = s.Id,
                        EmployeeId = employee.Id
                    });

                    _context.EmployeeSubdivisions.AddRange(newSubdivisionList);

                    await _context.SaveChangesAsync();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    throw new Exception(ex.Message);
                }
            }
        }

        ///<inheritdoc/>
        public async Task DeleteEmployeeAsync(int id)
        {
            if (id != 1)
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        var employee = await _context.Employees.Include(u => u.Account).FirstOrDefaultAsync(u => u.Id == id);

                        var employeeAccaunt = await _context.Accounts.FirstOrDefaultAsync(a => a.EmployeeId == id);

                        var employeeRoles = await _context.EmployeeRoles.Where(ur => ur.EmployeeId == id).ToListAsync();

                        _context.EmployeeRoles.RemoveRange(employeeRoles);

                        _context.Accounts.Remove(employeeAccaunt);

                        _context.Employees.Remove(employee);

                        _context.SaveChanges();

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();

                        throw new Exception(ex.Message);
                    }
                }
            }
            else
            {
                throw new Exception("Нельзя удалить администратора сиистемы!");
            }
        }
    }
}
