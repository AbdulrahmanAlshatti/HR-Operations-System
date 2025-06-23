using HR_Operations_System.Business;
using HR_Operations_System.Data;
using HR_Operations_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HR_Operations_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private IRepository _rep;
        private readonly ILogger<EmployeesController> _logger;

        private readonly AppDbContext _context;
        public EmployeesController(AppDbContext context, IRepository rep, ILogger<EmployeesController> logger)
        {
            _context = context;
            _rep = rep;
            _logger = logger;
        }

        [HttpGet("all")]
        public IActionResult GetAllEmployees()
        {
            var employees = _context.Employees.Include(x => x.TimingPlan).Include(x => x.ApplicationUser).ToList();

            var x = employees
            .Select(x => new
            {
                x.Id,
                Email = x.ApplicationUser != null ? x.ApplicationUser.Email : "",
                x.FingerCode,
                x.NameE,
                x.NameA,
                x.DeptCode,
                TimingPlan = GetTimingPlanFromEmployee(x),
                x.JobType,
                x.Sex,
                x.CheckLate,
                x.HasAllow,
                x.IsActive,
            }).ToList();
            return Ok(x);
        }

        public TimingPlan GetTimingPlanFromEmployee(Employee employee)
        {
            if (!employee.HasAllow)
                return NewMethod(employee.TimingPlan);

            var a = _context.EmployeeAllows.FirstOrDefault(x => x.EmpId == employee.Id);

            if (a == null)
                return NewMethod(employee.TimingPlan);

            return _context.TimingPlans.FirstOrDefault(x => x.Id == a.TimingCode);
            //return NewMethod(a.TimingPlan);
        }

        static TimingPlan NewMethod(TimingPlan res)
        {
            if (res != null) res.Employees = null;
            return res;
        }

        [HttpGet("all/{id}")]
        public IActionResult GetEmployee(int id)
        {
            var employees = _context.Employees.Include(x => x.TimingPlan).Include(x => x.ApplicationUser).ToList();

            var x = employees
            .Select(x => new
            {
                x.Id,
                Email = x.ApplicationUser != null ? x.ApplicationUser.Email : "",
                x.FingerCode,
                x.NameE,
                x.NameA,
                x.DeptCode,
                TimingPlan = GetTimingPlanFromEmployee(x),
                x.JobType,
                x.Sex,
                x.CheckLate,
                x.HasAllow,
                x.IsActive,
            }).ToList();

            var res = x.FirstOrDefault(x => x.Id == id);
            return Ok(res);
        }

        public class UpdateEmployeeRequestBody
        {
            public int Id { get; set; }
            public int TimingCode { get; set; }
        }

        [HttpPost]
        [Route("UpdateEmployee")]
        public async Task<ActionResult> UpdateEmployee(UpdateEmployeeRequestBody entity)
        {
            if (ModelState.IsValid)
            {
                await _rep.UpdateAsync<Employee>(entity.Id, record =>
                {
                    record.TimingCode = entity.TimingCode == 0 ? record.TimingCode : entity.TimingCode;
                    return Task.CompletedTask;
                });

                return Ok(new { message = "Employee has been updated successfully." });
            }
            else
            {
                return BadRequest();
            }
        }

    }


}
