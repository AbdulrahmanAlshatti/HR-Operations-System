using HR_Operations_System.Business;
using HR_Operations_System.Data;
using HR_Operations_System.Models;
using HR_Operations_System.Models.RequestModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Data;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        private IJwtService _jwtService;

        private readonly AppDbContext _context;

        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<AppUser> userManager,
            SignInManager<AppUser> signinManager,
            RoleManager<IdentityRole> roleManager,
            AppDbContext context,
            ILogger<AccountController> logger,
            IRepository rep,
            IJwtService jwtService)
        {
            _context = context;
            _userManager = userManager;
            _signinManager = signinManager;
            _roleManager = roleManager;
            _rep = rep;
            _logger = logger;
            _jwtService = jwtService;
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
                var roleResult = await _userManager.AddToRoleAsync(user, data.Role);
                if (roleResult.Errors.Any())
                {
                    return BadRequest(roleResult.Errors);
                }

                var isAdded = await _rep.AddAsync<Employee>(new Employee
                {
                    UserId = user.Id,
                    FingerCode = await GenerateFingerCode(),
                    DeptCode = data.DeptCode,
                    NameA = $"{user.FirstName} {user.LastName}",
                    NameE = "Placeholder",
                    TimingCode = data.TimingCode,
                    JobType = 1,
                    Sex = data.Gender,
                    HasAllow = false,
                    CheckLate = true,
                    IsActive = true
                });
                if (isAdded == true)
                {
                    emailrequest.SendEmail(data.Email, "Credentials", $"Dear {prefix}.{data.LastName}, \n Your Organization Credentials are: \nUsername: {data.Email} \nPassword: {password}");
                    return Ok(new { message = "Registered." });
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
        [HttpPost]
        [Route("AddRole")]
        public async Task<ActionResult> AddRole(string roleName)
        {
            IdentityRole role = new IdentityRole
            {
                Name = roleName
            };
            var result = await _roleManager.CreateAsync(role);
            if (result.Succeeded)
            {
                return Ok("Role Successfully Created");
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _signinManager.PasswordSignInAsync(request.Username, request.Password, false, false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(request.Username);
                var emp = await _rep.GetByAsync<Employee>(emp => emp.UserId == user.Id);
                emp.TimingPlan = await _rep.GetByIdAsync<TimingPlan>(emp.TimingCode);
                var token = await _jwtService.GenerateTokenAsync(user, emp);
                return Ok(new { token });
            }

            return Unauthorized("Invalid credentials");
        }

        [HttpPost]
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
        {
            var user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null) return NotFound();

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // Mark first login as complete
            user.IsFirstLogin = false;
            await _userManager.UpdateAsync(user);

            return Ok("Password changed successfully.");
        }
        [NonAction]
        private async Task<int> GenerateFingerCode()
        {
            int _min = 0000;
            int _max = 9999;
            Random _rdm = new Random();

            var fingerCodes = (await _rep.GetAsync<Employee>()).Select(c => c.FingerCode);
            int generatedCode = _rdm.Next(_min, _max);
            while (fingerCodes.Contains(generatedCode))
            {
                generatedCode = _rdm.Next(_min, _max);
            }
            return generatedCode;
        }

        private string GeneratePassword(int length = 8)
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
















        public async void addRandomData()
        {
            string password = "12345@Abc";

            _context.Locations.Add(new Location()
            {
                DescA = "الرئيسي",
                DescE = "HQ Building",
            });

            _context.Locations.Add(new Location()
            {
                DescA = "مشرف",
                DescE = "Mishrif",
            });

            _context.Locations.Add(new Location()
            {
                DescA = "جنوب السرة",
                DescE = "South Surra",
            });

            _context.Locations.Add(new Location()
            {
                DescA = "فهد الأحمد",
                DescE = "Fahad Al-Ahmad",
            });

            _context.Nodes.Add(new Node()
            {
                SerialNo = "SERIAL_MAIN_GROUND",
                DescA = "جهاز دور الأرضي",
                DescE = "HQ G",
                LocCode = 1,
                Floor = "G",
            });

            for (int i = 1; i < 22; i++)
            {

                _context.Nodes.Add(new Node()
                {
                    SerialNo = $"SERIAL_MAIN_FLOOR{i}-1",
                    DescA = $"جهاز الرئيسي دور {i}-1",
                    DescE = $"HQ Floor {i}-1",
                    LocCode = 1,
                    Floor = $"{i}",
                });

                _context.Nodes.Add(new Node()
                {
                    SerialNo = $"SERIAL_MAIN_FLOOR{i}-2",
                    DescA = $"جهاز الرئيسي دور {i}-2",
                    DescE = $"HQ Floor {i}-2",
                    LocCode = 1,
                    Floor = $"{i}",
                });

            }

            _context.Nodes.Add(new Node()
            {
                SerialNo = "SERIAL_MISHRIF_1",
                DescA = "جهاز مشرف 1",
                DescE = "Mishrif 1",
                LocCode = 2,
                Floor = "G",
            });

            _context.Nodes.Add(new Node()
            {
                SerialNo = "SERIAL_MISHRIF_2",
                DescA = "جهاز مشرف 2",
                DescE = "Mishrif 2",
                LocCode = 2,
                Floor = "G",
            });



            _context.Nodes.Add(new Node()
            {
                SerialNo = "SERIAL_SOUTH_SURRA_1",
                DescA = "جهاز جنوب السرة 1",
                DescE = "South Surra 1",
                LocCode = 3,
                Floor = "G",
            });

            _context.Nodes.Add(new Node()
            {
                SerialNo = "SERIAL_SOUTH_SURRA_2",
                DescA = "جهاز جنوب السرة 2",
                DescE = "South Surra 2",
                LocCode = 3,
                Floor = "G",
            });

            _context.Nodes.Add(new Node()
            {
                SerialNo = "SERIAL_FAHAD_ALAHMAD_1",
                DescA = "جهاز فهد الأحمد 1",
                DescE = "Fahad Al-Ahmad 1",
                LocCode = 4,
                Floor = "G",
            });

            _context.Nodes.Add(new Node()
            {
                SerialNo = "SERIAL_FAHAD_ALAHMAD_2",
                DescA = "جهاز فهد الأحمد 2",
                DescE = "Fahad Al-Ahmad 2",
                LocCode = 4,
                Floor = "G",
            });




            _context.Departments.Add(new Department()
            {
                DescA = "إدارة الأشتراكات",
                DescE = "ASDF",
                IsActive = true,
                DeptCode = 345,
            });

            _context.Departments.Add(new Department()
            {
                DescA = "إدارة الأنظمة والتطبيقات الذكية",
                DescE = "Smart Systems",
                IsActive = true,
                DeptCode = 812,
            });

            _context.TimingPlans.Add(new TimingPlan()
            {
                DescA = "صباحي",
                DescE = "Morning",
                FromTime = new TimeSpan(7, 45, 00),
                ToTime = new TimeSpan(14, 00, 00),
                RmdFromTime = new TimeSpan(8, 45, 00),
                RmdToTime = new TimeSpan(14, 00, 00),
                IsRamadan = false,
                IsAllow = false,
            });

            _context.TimingPlans.Add(new TimingPlan()
            {
                DescA = "مسائي",
                DescE = "Night",
                FromTime = new TimeSpan(17, 0, 00),
                ToTime = new TimeSpan(21, 00, 00),
                RmdFromTime = new TimeSpan(17, 30, 00),
                RmdToTime = new TimeSpan(20, 30, 00),
                IsRamadan = false,
                IsAllow = false,
            });

            _context.TimingPlans.Add(new TimingPlan()
            {
                DescA = "تخفيف أمومة",
                DescE = "Maternal Reduction",
                FromTime = new TimeSpan(9, 00, 00),
                ToTime = new TimeSpan(13, 00, 00),
                RmdFromTime = new TimeSpan(9, 30, 00),
                RmdToTime = new TimeSpan(12, 30, 00),
                IsRamadan = false,
                IsAllow = true,
            });

            _context.TimingPlans.Add(new TimingPlan()
            {
                DescA = "تخفيف طبي",
                DescE = "Medical Reduction",
                FromTime = new TimeSpan(9, 30, 00),
                ToTime = new TimeSpan(13, 00, 00),
                RmdFromTime = new TimeSpan(10, 00, 00),
                RmdToTime = new TimeSpan(12, 30, 00),
                IsRamadan = false,
                IsAllow = true,
            });


            _context.SaveChanges();

            List<(string, string, string, string, int)> emailList = [
                ("abdulrahmanalshatti@mail.com", "عبدالرحمن", "الشطي","Admin",1),
                ("abdulrahmanalkandari@mail.com", "عبدالرحمن", "الكندري","Admin",1),
                ("zalhallaq@mail.com", "زينب", "الحلاق","Employee",2),
                ("beshow95@mail.com", "بشاير", "الخالدي","Employee",2),
            ];


            await _roleManager.CreateAsync(new IdentityRole { Name = "Employee" });

            await _roleManager.CreateAsync(new IdentityRole { Name = "Admin" });

            foreach (var data in emailList)
            {
                AppUser user = new AppUser
                {
                    UserName = data.Item1,
                    Email = data.Item1,
                    FirstName = data.Item2,
                    LastName = data.Item3,
                    IsFirstLogin = true
                };
                var result = await _userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(user, data.Item4);

                    var isAdded = await _rep.AddAsync<Employee>(new Employee
                    {
                        UserId = user.Id,
                        FingerCode = await GenerateFingerCode(),
                        DeptCode = 812,
                        NameA = $"{user.FirstName} {user.LastName}",
                        NameE = "Placeholder",
                        TimingCode = 1,
                        JobType = 1,
                        Sex = data.Item5,
                        HasAllow = false,
                        CheckLate = true,
                        IsActive = true
                    });
                }
            }

            for (int i = 0; i < 4; i++)
                AddDummyData(i);

        }


        public async void AddDummyData(int FingerCode)
        {
            System.Random r = new System.Random();
            List<Node> nodeList = (await _rep.GetAsync<Node>()).ToList();

            DateTime startDate = new DateTime(2025, 4, 1);
            DateTime endDate = new DateTime(2025, 6, 30);

            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                if (date.DayOfWeek == DayOfWeek.Friday || date.DayOfWeek == DayOfWeek.Saturday)
                    continue;

                if (r.NextDouble() < 0.05)
                    continue;

                if (r.NextDouble() < 0.95)
                {
                    Attendance enter = new Attendance()
                    {
                        FingerCode = FingerCode,
                        IODateTime = date.AddMinutes(r.Next(360, 540)),
                        NodeSerialNo = nodeList[r.Next(nodeList.Count)].SerialNo,
                        IsActive = true,
                        TrType = 0,
                    };
                    await _rep.AddAsync(enter);
                }

                if (r.NextDouble() < 0.1)
                {
                    Attendance excuseLeave = new Attendance()
                    {
                        FingerCode = FingerCode,
                        IODateTime = date.AddMinutes(r.Next(570, 670)),
                        NodeSerialNo = nodeList[r.Next(nodeList.Count)].SerialNo,
                        IsActive = true,
                        TrType = 1,
                    };

                    Attendance excuseEnter = new Attendance()
                    {
                        FingerCode = FingerCode,
                        IODateTime = date.AddMinutes(r.Next(680, 780)),
                        NodeSerialNo = nodeList[r.Next(nodeList.Count)].SerialNo,
                        IsActive = true,
                        TrType = 0,
                    };

                    await _rep.AddAsync(excuseLeave);
                    await _rep.AddAsync(excuseEnter);
                }


                if (r.NextDouble() < 0.95)
                {
                    Attendance leave = new Attendance()
                    {
                        FingerCode = FingerCode,
                        IODateTime = date.AddMinutes(r.Next(810, 870)),
                        NodeSerialNo = nodeList[r.Next(nodeList.Count)].SerialNo,
                        IsActive = true,
                        TrType = 1,
                    };
                    await _rep.AddAsync(leave);
                }
            }
        }



















    }
}
