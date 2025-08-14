using Fastdotnet.Core.Entities.Admin;
using Fastdotnet.Core.Entities.App;
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
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Fastdotnet.Service.Service
{
    public class AuthService : IAuthService
    {
        private readonly IRepository<FdAdminUser> _adminUserRepository;
        private readonly IRepository<FdAppUser> _appUserRepository;
        private readonly JwtSettings _jwtSettings;

        public AuthService(
            IRepository<FdAdminUser> adminUserRepository,
            IRepository<FdAppUser> appUserRepository,
            IOptions<JwtSettings> jwtSettings)
        {
            _adminUserRepository = adminUserRepository;
            _appUserRepository = appUserRepository;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<string> LoginAsync(LoginDto dto, string userCategory)
        {
            long userId;
            string userName;
            // In a real application, you would also fetch user roles to include in the token.

            if (userCategory == "Admin")
            {
                var user = await _adminUserRepository.GetFirstAsync(u => u.Username == dto.Username);
                if (user == null || user.Password != dto.Password) // IMPORTANT: Replace with a proper password hash check
                {
                    throw new BusinessException("用户名或密码错误");
                }
                userId = user.Id;
                userName = user.Username;
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
            }
            else
            {
                throw new BusinessException("无效的用户类别");
            }

            return GenerateJwtToken(userId, userName, userCategory);
        }

        private string GenerateJwtToken(long userId, string userName, string category)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, userName),
                new Claim("category", category) // Custom claim for user category
                // Add other claims like roles here
            };

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
