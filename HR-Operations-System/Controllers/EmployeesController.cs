using HR_Operations_System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HR_Operations_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly AppDbContext _context;
        public EmployeesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("all")]
        public IActionResult GetAllEmployees()
        {
            var employees = _context.Employees.Select(x => new
            {
                x.Id,
                Email = x.ApplicationUser == null ? "" : x.ApplicationUser.Email,
                x.FingerCode,
                x.NameE,
                x.NameA,
                x.DeptCode,
                x.TimingPlan,
                x.JobType,
                x.Sex,
                x.CheckLate,
                x.HasAllow,
                x.IsActive,
            });
            return Ok(employees);
        }

    }


}
