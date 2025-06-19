using HR_Operations_System.Business;
using HR_Operations_System.Data;
using HR_Operations_System.Models;
using HR_Operations_System.Models.RequestModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Runtime.InteropServices;
using System.Text;

namespace HR_Operations_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private UserManager<AppUser> _userManager;
        private SignInManager<AppUser> _signinManager;
        private RoleManager<IdentityRole> _roleManager;
        private IRepository _rep;

        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<AppUser> userManager,
            SignInManager<AppUser> signinManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<AccountController> logger,
            IRepository rep)
        {
            _userManager = userManager;
            _signinManager = signinManager;
            _roleManager = roleManager;
            _rep = rep;
            _logger = logger;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<ActionResult> Register(RegisterRequestModel data)
        {
            Email emailrequest = new Email("hroperationscodedcap@gmail.com", "gegw zwwq mevo fnfq");
            string password = GeneratePassword();
            AppUser user = new AppUser
            {
                UserName = data.Email,
                Email = data.Email,
                FirstName = data.FirstName,
                LastName = data.LastName,
                IsFirstLogin = true
            };
            string prefix = data.Gender == 1 ? "Mr" : "Ms";
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                var isAdded = await _rep.AddAsync<Employee>(new Employee
                {
                    UserId = user.Id,
                    FingerCode = await GenerateFingerCode(),
                    DeptCode = data.DeptCode,
                    NameA = "",
                    NameE = $"{user.FirstName} {user.LastName}",
                    TimingCode = data.TimingCode,
                    JobType = 1,
                    Sex = data.Gender,
                    HasAllow = false,
                    CheckLate = true,
                    IsActive = true
                });
                if(isAdded == true)
                {
                    emailrequest.SendEmail(data.Email, "Credentials", $"Dear {prefix}.{data.LastName}, \n Your Organization Credentials are: \nUsername: {data.Email} \nPassword: {password}");
                    return Ok();
                }
                //emailrequest.SendEmail(data.Email, "Credentials", $"Dear {prefix}.{data.LastName}, \n Your Organization Credentials are: \nUsername: {data.Email} \nPassword: {password}");
                return BadRequest();
            }
            else
            {
                //emailrequest.SendEmail(data.Email, "Credentials", $"Dear {prefix}.{data.LastName}, \n Your Organization Credentials are: \nUsername: {data.Email} \nPassword: {password}");
                return BadRequest(result.Errors);
            }

        }

        public async Task<int> GenerateFingerCode()
        {
            int _min = 0000;
            int _max = 9999;
            Random _rdm = new Random();

            var fingerCodes = (await _rep.GetAsync<Employee>()).Select(c => c.FingerCode);
            int generatedCode = _rdm.Next(_min, _max);
            while(fingerCodes.Contains(generatedCode)){
                generatedCode = _rdm.Next(_min, _max);
            }
            return generatedCode;
        }
        public static string GeneratePassword(int length = 8)
        {
            const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lower = "abcdefghijklmnopqrstuvwxyz";
            const string digits = "0123456789";
            const string special = "!@#$%^&*()-_=+[]{}|;:,.<>?";

            Random random = new Random();

            // Ensure the password has at least one of each required character type
            StringBuilder password = new StringBuilder();
            password.Append(upper[random.Next(upper.Length)]);
            password.Append(lower[random.Next(lower.Length)]);
            password.Append(digits[random.Next(digits.Length)]);
            password.Append(special[random.Next(special.Length)]);

            // Fill the remaining characters randomly from all categories
            string allChars = upper + lower + digits + special;
            for (int i = 4; i < length; i++)
            {
                password.Append(allChars[random.Next(allChars.Length)]);
            }

            // Shuffle the result to prevent predictable character positions
            return new string(password.ToString().OrderBy(c => random.Next()).ToArray());
        }

    }
}
