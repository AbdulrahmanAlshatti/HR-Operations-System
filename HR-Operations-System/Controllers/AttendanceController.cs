using HR_Operations_System.Business;
using HR_Operations_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HR_Operations_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendancesController : ControllerBase
    {
        private IRepository _rep;
        private readonly ILogger<AttendancesController> _logger;

        public AttendancesController(IRepository rep, ILogger<AttendancesController> logger)
        {
            _rep = rep;
            _logger = logger;
        }

        [HttpGet]
        [Route("GetAttendance")]
        public async Task<ActionResult<IEnumerable<Attendance>>> GetAttendance()
        {
            var result = await _rep.GetAsync<Attendance>();
            return Ok(result);
        }


        [HttpPost]
        [Route("AddAttendance")]
        public async Task<ActionResult> AddAttendance(Attendance entity)
        {
            await _rep.AddAsync(entity);
            return Ok();
        }

        public class GetAttendanceDay
        {
            public DateOnly Day { get; set; }
            public int FingerCode { get; set; }
        }

        [HttpPost]
        [Route("GetAttendanceOfDay")]
        public async Task<ActionResult<IEnumerable<Attendance>>> GetAttendanceOfDay(GetAttendanceDay getAttendanceDay)
        {
            DateTime dateTime = getAttendanceDay.Day.ToDateTime(new TimeOnly());
            var result = await _rep.GetListByAsync<Attendance>(x => x.FingerCode == getAttendanceDay.FingerCode && x.IODateTIme.Date == dateTime);
            return Ok(result);
        }


    }


}
