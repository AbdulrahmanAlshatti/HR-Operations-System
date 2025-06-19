using HR_Operations_System.Business;
using HR_Operations_System.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HR_Operations_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private IRepository _rep;
        private readonly ILogger<DepartmentsController> _logger;

        public DepartmentsController(IRepository rep, ILogger<DepartmentsController> logger)
        {
            _rep = rep;
            _logger = logger;
        }

        [HttpGet]
        [Route("GetDepartments")]
        public async Task<ActionResult<IEnumerable<Department>>> GetDepartments()
        {
            var result = await _rep.GetAsync<Department>();
            return Ok(result);
        }
    }
}
