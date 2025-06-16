using HR_Operations_System.Business;
using HR_Operations_System.Models;
using Microsoft.AspNetCore.Mvc;

namespace HR_Operations_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendancesController : ControllerBase
    {
        private IRepository _rep;
        private readonly ILogger<AttendancesController> _logger;

        public AttendancesController(IRepository rep, ILogger<AttendancesController> logger )
        {
            _rep = rep;
            _logger = logger;
        }

        [HttpGet]
        [Route("GetAttendance")]
        public async Task<ActionResult<IEnumerable<Attendance>>> GetAttendance()
        {
            var a= await _rep.GetAsync<Attendance>();
            return Ok(a);
        }
    }


}
