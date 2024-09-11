using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using CarRentalApi.Data;
using CarRentalApi.Models;

namespace CareRentalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InitController : ControllerBase
    {
        private readonly CarRentalContext _context;

        public InitController(CarRentalContext context)
        {
            _context = context;
        }

        // GET: api/Car
        [HttpGet]
        public async Task<IActionResult> RunInit()
        {

            var make1 = new Make { Name = "Jeep" };
            var make2 = new Make { Name = "Toyota" };
            var make3 = new Make { Name = "Audi" };

            await _context.Makes.AddAsync(make1);
            await _context.Makes.AddAsync(make2);
            await _context.Makes.AddAsync(make3);

            var car1 = new Car
            {
                MakeId = make1.Id,
                Make = make1,
                Model = "Wrangler",
                Description = "Some description",
                Deposit = 250m,
                Seats = 5,
                Doors = 4,
                TransmissionType = TransmissionType.auto,

                PowerHp = 280,
                RangeKm = 650,
                Available = true,
            };

            var car2 = new Car
            {
                MakeId = make2.Id,
                Make = make2,
                Model = "Prado",
                Description = "Some description",
                Deposit = 250m,
                Seats = 5,
                Doors = 4,
                TransmissionType = TransmissionType.auto,

                PowerHp = 320,
                RangeKm = 600,
                Available = true,
            };

            await _context.Cars.AddAsync(car1);
            await _context.Cars.AddAsync(car2);

            var location1 = new Location
            {
                Name = "Central London Office",
                PickUpAvailable = true,
                DropOffAvailable = true
            };

            var location2 = new Location
            {
                Name = "Heathrow Airport",
                PickUpAvailable = true,
                DropOffAvailable = true
            };

            await _context.Locations.AddAsync(location1);
            await _context.Locations.AddAsync(location2);

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
