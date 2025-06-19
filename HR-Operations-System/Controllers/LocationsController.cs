using HR_Operations_System.Business;
using HR_Operations_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq.Expressions;

namespace HR_Operations_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationsController : ControllerBase
    {
        private IRepository _rep;
        private readonly ILogger<LocationsController> _logger;

        public LocationsController(IRepository rep, ILogger<LocationsController> logger)
        {
            _rep = rep;
            _logger = logger;
        }
        [HttpGet]
        [Route("GetLocations")]
        public async Task<ActionResult<IEnumerable<Location>>> GetLocations()
        {
            var locations = await _rep.GetAsync<Location>();
            return Ok(locations);
        }
        [HttpPost]
        [Route("AddLocations")]
        public async Task<ActionResult> AddLocations(Location entity)
        {
            await _rep.AddAsync<Location>(entity);
            return Ok("Location Has Been Added");
        }
        [HttpDelete]
        [Route("DeleteLocations/{id}")]
        public async Task<ActionResult> DeleteLocations(int id )
        {
            await _rep.DeleteAsync<Location>(c => c.Id==id);
            return Ok("Location Has Been Deleted Succesfully");
        }
        [HttpPut]
        [Route("UpdateLocations")]
        public async Task<ActionResult<Location>> UpdateLocations(Location entity)
        {
            await _rep.UpdateAsync<Location>(entity.Id, record => {
                record.DescA = entity.DescA == "" ? record.DescA : entity.DescA;
                record.DescE = entity.DescE=="" ? record.DescE:entity.DescE;
                return Task.CompletedTask;
            });
            return Ok(_rep.GetByIdAsync<Location>(entity.Id));
        }
    }
    

}
