using HR_Operations_System.Business;
using HR_Operations_System.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PdfReportGenerator;

namespace HR_Operations_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeAllowsController : ControllerBase
    {
        private IRepository _repo;
        private readonly ILogger<EmployeeAllowsController> _logger;

        public EmployeeAllowsController (IRepository repo, ILogger<EmployeeAllowsController> logger)
        {
            _repo = repo;
            _logger = logger;
        }
        [HttpGet]
        [Route("GetEmployeeAllows")]
        public async Task<ActionResult<IEnumerable<EmployeeAllow>>> GetEmployeeAllows()
        {
            var AllowList = await _repo.GetAsync<EmployeeAllow>();
            return Ok(AllowList);
        }
        [HttpGet]
        [Route("GetEmployeeAllowsById/{id}")]
        public async Task<ActionResult<IEnumerable<EmployeeAllow>>> GetEmployeeAllowsById(int id)
        {
            var AllowList = await _repo.GetByAsync<EmployeeAllow>(c => c.EmpId == id);
            return Ok(AllowList);
        }
        [HttpPost]
        [Route("AddEmployeeAllow")]
        public async Task<ActionResult<IEnumerable<EmployeeAllow>>> GetEmployeeAllowsById(EmployeeAllow entity)
        {
            await _repo.AddAsync<EmployeeAllow>(entity);
            return Ok("Employee Has been added");
        }
        [HttpDelete]
        [Route("EndAllow")]
        public async Task<ActionResult> EndAllow(int id)
        {

            await _repo.UpdateAsync<EmployeeAllow>(id, c => {
                c.Status = false;
                return Task.CompletedTask;
                });
            return Ok();

        }
        //[HttpPost]
        //[Route("EndAllow")]
        //public async Task<ActionResult> EndAllow(int id, DateTime date)
        //{
        //    var AllowList = await _repo.GetByAsync<EmployeeAllow>(c => c.Id == id);
        //    var newEmpAllow = new EmployeeAllow();
        //    await _repo.UpdateAsync<EmployeeAllow>(Id, c => c.Id == id)
        //}
    }
}

