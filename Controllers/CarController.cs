using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

using CarRentalApi.Data;
using CarRentalApi.Models;

namespace CarRentalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarController : ControllerBase
    {
        private readonly CarRentalContext _context;
        private readonly CarService _service;

        public CarController(CarRentalContext context, CarService service)
        {
            _context = context;
            _service = service;
        }

        // GET: api/Car
        [HttpGet]
        public async Task<IEnumerable<Car>> GetCars()
        {
            // var audience = User.Claims;
            // foreach (var i in audience)
            // {
            //     Console.WriteLine(i);
            // }
            // var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // Console.WriteLine("User ID: " + userId);
            return await _service.GetAll();
        }

        // Get: api/Car/1/2/2011-10-05T14:48:00.000Z/2011-10-05T14:48:00.000Z
        [HttpGet("{pickUpLocationId}/{dropOffLocationId}/{pickUpDateTimeIso}/{dropOffDateTimeIso}")]
        public async Task<IEnumerable<Car>> GetAvailableCars(long pickUpLocationId, long dropOffLocationId, string pickUpDateTimeIso, string dropOffDateTimeIso)
        {
            return await _service.GetAvailableCars(pickUpLocationId, dropOffLocationId, pickUpDateTimeIso, dropOffDateTimeIso);
        }

        // GET: api/Car/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Car>> GetCar(long id)
        {
            var car = await _service.GetById(id);

            if (car == null)
            {
                return NotFound();
            }

            return car;
        }

        // PUT: api/Car/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCar(long id, Car car)
        {
            var make = await _context.Makes.SingleOrDefaultAsync(m => m.Id == car.MakeId);

            if (id != car.Id || make == null)
            {
                return BadRequest();
            }

            bool carModified = await _service.Update(id, car);

            if (!carModified)
            {
                return NotFound();
            }



            return NoContent();
        }

        // POST: api/Car
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Car>> PostCar(Car car)
        {
            var updatedCar = await _service.Create(car);
            if (updatedCar == null)
            {
                return BadRequest();
            }

            return CreatedAtAction(nameof(GetCar), new { id = updatedCar.Id }, updatedCar);
        }

        // DELETE: api/Car/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCar(long id)
        {
            var car = await _service.GetById(id);
            if (car == null)
            {
                return NotFound();
            }

            await _service.Delete(car);

            return NoContent();
        }
    }
}
