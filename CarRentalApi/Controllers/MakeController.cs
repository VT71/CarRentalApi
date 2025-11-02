using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarRentalApi.Models;
using CarRentalApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace CarRentalApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MakeController : ControllerBase
    {
        private readonly IMakeService _service;

        public MakeController(IMakeService service)
        {
            _service = service;
        }

        // GET: api/Make
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Make>>> GetMakes()
        {

            return Ok(await _service.GetAll());
        }

        // GET: api/Make/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Make>> GetMake(long id)
        {
            var make = await _service.GetById(id);

            if (make == null)
            {
                return NotFound();
            }

            return Ok(make);
        }

        // POST: api/Make
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Make>> PostMake(Make make)
        {
            var createdMake = await _service.Create(make);
            return CreatedAtAction(nameof(GetMake), new { id = createdMake.Id }, createdMake);
        }

        // PUT: api/Make/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMake(long id, Make make)
        {
            try
            {
                var makeUpdated = await _service.Update(id, make);

                if (makeUpdated == false)
                {
                    return BadRequest();
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await MakeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Make/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMake(long id)
        {
            var makeDeleted = await _service.Delete(id);
            if (!makeDeleted) { return NotFound(); }

            return NoContent();
        }

        private async Task<bool> MakeExists(long id)
        {
            return await _service.GetById(id) != null;
        }
    }
}
