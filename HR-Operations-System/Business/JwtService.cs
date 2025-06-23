using HR_Operations_System.Data;
using HR_Operations_System.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Drawing;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace HR_Operations_System.Business
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _config;
        private UserManager<AppUser> _userManager;
        public JwtService(IConfiguration config, UserManager<AppUser> roleManager)
        {
            _config = config;
            _userManager = roleManager;
        }

        string GetTimeString(TimeSpan time)
        {
            DateTime dateTime = DateTime.Today.Add(time);
            return dateTime.ToString("hh:mm tt");
        }
        public Task<string> GenerateTokenAsync(AppUser user, Employee emp)
        {
            var userInfo = new
            {
                userName = user.UserName,
                firstName = user.FirstName,
                lastName = user.LastName,
                email = user.Email,
                deptCode = user.Employee.DeptCode,
                fingerCode = user.Employee.FingerCode,
                fromTime = GetTimeString(user.Employee.TimingPlan.FromTime),
                toTime = GetTimeString(user.Employee.TimingPlan.ToTime),
            };

            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim("fingerCode", emp.FingerCode.ToString()),
            new Claim("empId", emp.Id.ToString()),
            new Claim("role",_userManager.GetRolesAsync(user).Result.First()),
            new Claim("userInfo", JsonSerializer.Serialize(userInfo)),
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
        }
    }
}
