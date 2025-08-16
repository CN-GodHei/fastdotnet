using Fastdotnet.Core.Entities.Admin;
using Fastdotnet.Core.Entities.App;
using Fastdotnet.Core.Entities.System;
using Fastdotnet.Core.Exceptions;
using Fastdotnet.Core.IService;
using Fastdotnet.Core.Models.Auth;
using Fastdotnet.Core.Settings;
using Fastdotnet.Service.IService;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Fastdotnet.Service.Service
{
    public class AuthService : IAuthService
    {
        private readonly IRepository<FdAdminUser> _adminUserRepository;
        private readonly IRepository<FdAppUser> _appUserRepository;
        private readonly IRepository<FdAdminUserRole> _adminUserRoleRepository;
        private readonly IRepository<FdAppUserRole> _appUserRoleRepository;
        private readonly IRepository<FdRole> _roleRepository;
        private readonly JwtSettings _jwtSettings;

        public AuthService(
            IRepository<FdAdminUser> adminUserRepository,
            IRepository<FdAppUser> appUserRepository,
            IOptions<JwtSettings> jwtSettings,
            IRepository<FdAdminUserRole> adminUserRoleRepository,
            IRepository<FdAppUserRole> appUserRoleRepository,
            IRepository<FdRole> roleRepository)
        {
            _adminUserRepository = adminUserRepository;
            _appUserRepository = appUserRepository;
            _jwtSettings = jwtSettings.Value;
            _adminUserRoleRepository = adminUserRoleRepository;
            _appUserRoleRepository = appUserRoleRepository;
            _roleRepository = roleRepository;
        }

        public async Task<string> LoginAsync(LoginDto dto, string userCategory)
        {
            string userId;
            string userName;
            List<string> roleCodes = new List<string>();

            if (userCategory == "Admin")
            {
                var user = await _adminUserRepository.GetFirstAsync(u => u.Username == dto.Username);
                if (user == null || user.Password != dto.Password) // IMPORTANT: Replace with a proper password hash check
                {
                    throw new BusinessException("用户名或密码错误");
                }
                userId = user.Id;
                userName = user.Username;

                var userRoles = await _adminUserRoleRepository.GetListAsync(ur => ur.AdminUserId == userId);
                var roleIds = userRoles.Select(ur => ur.RoleId).ToList();
                if (roleIds.Any())
                {
                    var roles = await _roleRepository.GetListAsync(r => roleIds.Contains(r.Id));
                    roleCodes.AddRange(roles.Select(r => r.Code));
                }
            }
            else if (userCategory == "App")
            {
                var user = await _appUserRepository.GetFirstAsync(u => u.Username == dto.Username);
                if (user == null || user.Password != dto.Password) // IMPORTANT: Replace with a proper password hash check
                {
                    throw new BusinessException("用户名或密码错误");
                }
                userId = user.Id;
                userName = user.Nickname;

                var userRoles = await _appUserRoleRepository.GetListAsync(ur => ur.AppUserId == userId);
                var roleIds = userRoles.Select(ur => ur.RoleId).ToList();
                if (roleIds.Any())
                {
                    var roles = await _roleRepository.GetListAsync(r => roleIds.Contains(r.Id));
                    roleCodes.AddRange(roles.Select(r => r.Code));
                }
            }
            else
            {
                throw new BusinessException("无效的用户类别");
            }

            return GenerateJwtToken(userId, userName, userCategory, roleCodes);
        }

        private string GenerateJwtToken(string userId, string userName, string category, List<string> roleCodes)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, userName),
                new Claim("category", category) // Custom claim for user category
            };

            // Add role claims
            foreach (var roleCode in roleCodes)
            {
                claims.Add(new Claim(ClaimTypes.Role, roleCode));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}