using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarRentalApi.Data;
using CarRentalApi.Models;
using CarRentalApi.Services;
using CarRentalApi.Services.Interfaces;

namespace CarRentalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly ILocationService _service;

        public LocationController(ILocationService service)
        {
            _service = service;
        }

        // GET: api/Location
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Location>>> GetLocations()
        {
            return Ok(await _service.GetAll());
        }

        //Get: api/Location/PickUp
        [HttpGet("PickUpLocations")]
        public async Task<ActionResult<IEnumerable<Location>>> GetPickUpLocations()
        {
            var locations = await _service.GetAllPickUp();
            return Ok(locations);
        }

        //Get: api/Location/DropOff
        [HttpGet("DropOffLocations")]
        public async Task<ActionResult<IEnumerable<Location>>> GetDropOffLocations()
        {
            var locations = await _service.GetAllDropOff();
            return Ok(locations);
        }

        // GET: api/Location/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Location>> GetLocation(long id)
        {
            var location = await _service.GetById(id);

            if (location == null)
            {
                return NotFound();
            }

            return Ok(location);
        }

        // PUT: api/Location/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLocation(long id, Location location)
        {
            if (id != location.Id)
            {
                return BadRequest();
            }

            bool locationUpdated = await _service.Update(id, location);

            if (!locationUpdated)
            {
                return NotFound();
            }

            return NoContent();

        }

        // POST: api/Location
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Location>> PostLocation(Location location)
        {
            Location newLocation = await _service.Create(location);

            return CreatedAtAction(nameof(GetLocation), new { id = newLocation.Id }, newLocation);
        }

        // DELETE: api/Location/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLocation(long id)
        {
            var location = await _service.GetById(id);
            if (location == null)
            {
                return NotFound();
            }

            await _service.Delete(location);

            return NoContent();
        }
    }
}
