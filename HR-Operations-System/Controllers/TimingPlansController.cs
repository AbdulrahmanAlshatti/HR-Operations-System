using HR_Operations_System.Business;
using HR_Operations_System.Models;
using Microsoft.AspNetCore.Mvc;

namespace HR_Operations_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimingPlansController : ControllerBase
    {
        private IRepository _rep;
        private readonly ILogger<TimingPlansController> _logger;

        public TimingPlansController(IRepository rep, ILogger<TimingPlansController> logger)
        {
            _rep = rep;
            _logger = logger;
        }

        [HttpGet]
        [Route("GetTimingPlan")]
        public async Task<ActionResult<IEnumerable<TimingPlan>>> GetTimingPlans()
        {
            var timingplans = await _rep.GetAsync<TimingPlan>();
            return Ok(timingplans);
        }
        [HttpGet]
        [Route("GetTimePlansNonAllow")]
        public async Task<ActionResult<IEnumerable<TimingPlan>>> GetTimePlansNonAllow()
        {
            var timingplans = await _rep.GetAsync<TimingPlan>();
            var timingPlansList = timingplans.Where(x => x.IsAllow == false).ToList();
            return Ok(timingPlansList);
            // return is allow false
        }
        [HttpGet]
        [Route("GetAllowTimePlan")]
        public async Task<ActionResult<IEnumerable<TimingPlan>>> GetAllowTimePlan()
        {
            // return is allow true
            var timingplans = await _rep.GetAsync<TimingPlan>();
            var timingPlansList = timingplans.Where(x => x.IsAllow == true).ToList();
            return Ok(timingPlansList);
        }

    }


}
