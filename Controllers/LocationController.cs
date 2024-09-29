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

namespace CareRentalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly CarRentalContext _context;
        private readonly LocationService _service;

        public LocationController(CarRentalContext context, LocationService service)
        {
            _context = context;
            _service = service;
        }

        // GET: api/Location
        [HttpGet]
        public async Task<IEnumerable<Location>> GetLocations()
        {
            return await _service.GetAll();
        }

        //Get: api/Location/PickUp
        public async Task<IEnumerable<Location>> GetPickUpLocations()
        {
            return await _service.GetAllPickUp();
        }

        //Get: api/Location/DropOff
        public async Task<IEnumerable<Location>> GetDropOffLocations()
        {
            return await _service.GetAllDropOff();
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

            return location;
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

            if (locationUpdated)
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
