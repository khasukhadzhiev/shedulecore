using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BL.ServiceInterface;
using DAL;
using DAL.Entities;
using DTL.Dto;
using Infrastructure.Helpers;
using Infrastructure.Models;

namespace BL.Services
{
    public class AccountService : IAccountService
    {
        private readonly ScheduleHighSchoolDb _context;

        private readonly ILogger<AccountService> _logger;

        public AccountService(ScheduleHighSchoolDb context, ILogger<AccountService> logger)
        {
            _context = context;
            _logger = logger;

            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        ///<inheritdoc/>
        public async Task<LoginResultDto> LogInAsync(AccountDto accountDto)
        {
            accountDto.Login = accountDto.Login.ToLower();

            var account = await _context.Accounts
                .Include(a => a.Employee)
                .FirstOrDefaultAsync(a => a.Login == accountDto.Login && a.Password == HashHelper.HashPassword(accountDto.Password));

            if (account == null)
            {
                return null;
            }

            if (!account.IsValid)
            {
                return new LoginResultDto
                {
                    IsValid = false,
                };
            }

            var employeeRoles = await _context.EmployeeRoles
                .Include(u => u.Role)
                .Where(ur => ur.EmployeeId == account.Employee.Id)
                .Select(r => r.Role.Name.ToLower())
                .ToListAsync();

            var identity = GetIdentity(account, employeeRoles);

            var now = DateTime.UtcNow;

            var jwToken = new JwtSecurityToken(
                    issuer: AuthorizeOptions.ISSUER,
                    audience: AuthorizeOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromHours(AuthorizeOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthorizeOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            _logger.LogWarning($@"Вход в систему: {JsonConvert.SerializeObject(
                new
                {
                    account.Employee.Id,
                    account.Employee.FirstName,
                    account.Employee.Name,
                    account.Employee.MiddleName,
                    account.IsValid,
                    account.Login
                },
                new JsonSerializerSettings { Formatting = Formatting.Indented })}");

            return new LoginResultDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(jwToken),
                EmployeeRoles = employeeRoles.ToArray(),
                FullName = $"{account.Employee.FirstName} {account.Employee.Name} {account.Employee.MiddleName}",
                IsAutentificated = true,
                Expiration = jwToken.ValidTo,
                IsValid = true,
            };
        }

        /// <summary>
        /// Генерация claims
        /// </summary>
        /// <param name="account">Аккаунт польователя</param>
        /// <param name="roles">Роли</param>
        /// <returns>Claims</returns>
        private ClaimsIdentity GetIdentity(Account account, List<string> roles)
        {
            if (account != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(CustomConstants.EMPLOYEE_ID_CLAIM_TYPE, account.EmployeeId.ToString()),
                    new Claim(ClaimsIdentity.DefaultNameClaimType, account.Login),
                    new Claim(ClaimsIdentity.DefaultNameClaimType, $"{account.Employee.FirstName} {account.Employee.Name} {account.Employee.MiddleName}"),
                };

                List<Claim> roleClaims = new List<Claim>();

                foreach (var role in roles)
                {
                    roleClaims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, role));
                }

                var resultClaims = claims.Concat(roleClaims);

                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(resultClaims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }

            return null;
        }
    }
}
