using HR_Operations_System.Business;
using HR_Operations_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace HR_Operations_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NodesController : ControllerBase
    {
        private IRepository _rep;
        private readonly ILogger<NodesController> _logger;

        public NodesController(IRepository rep, ILogger<NodesController> logger)
        {
            _rep = rep;
            _logger = logger;
        }
        [HttpGet]
        [Route("GetNodes")]
        public async Task<ActionResult<IEnumerable<Node>>> GetNodes()
        {
            var locations = (await _rep.GetAsync<Node>()).Select(x => new
            {
                x.SerialNo,
                x.DescA,
                x.DescE,
                x.Location,
                x.Floor,
            });
            return Ok(locations);
        }
        [HttpPost]
        [Route("AddNode")]
        public async Task<ActionResult> AddNode(Node entity)
        {
            await _rep.AddAsync<Node>(entity);
            return Ok(new { message = "Location Has Been Added" });
        }
        [HttpDelete]
        [Route("DeleteNode/{id}")]
        public async Task<ActionResult> DeleteNode(int id)
        {
            await _rep.DeleteAsync<Location>(c => c.Id == id);
            return Ok(new { message = "Location Has Been Deleted Succesfully" });
        }
        [HttpPut]
        [Route("UpdateNode")]
        public async Task<ActionResult<Node>> UpdateNode(Node entity)
        {
            if (ModelState.IsValid)
            {
                await _rep.UpdateAsync<Node>(entity.SerialNo, record =>
                {
                    record.DescA = entity.DescA == "" ? record.DescA : entity.DescA;
                    record.DescE = entity.DescE == "" ? record.DescE : entity.DescE;
                    record.LocCode = entity.LocCode == 0 ? record.LocCode : entity.LocCode;
                    record.Floor = entity.Floor == "" ? record.Floor : entity.Floor;
                    return Task.CompletedTask;
                });

                return Ok(_rep.GetByIdAsync<Node>(entity.SerialNo));
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet("CheckIdExists/{serialNo}")]
        public async Task<IActionResult> CheckIdExists(string serialNo)
      {
            bool exists = await _rep.AnyAsync<Node>(x => x.SerialNo == serialNo);
            return Ok(exists);
        }
    }
}



