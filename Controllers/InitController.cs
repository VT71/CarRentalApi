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
                TransmissionType = TransmissionType.Auto,

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
                TransmissionType = TransmissionType.Auto,

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

            var booking1 = new Booking
            {
                UserId = "1",
                CarId = car1.Id,
                Car = car1,
                PickUpDateTime = DateTimeOffset.Parse("2024-09-22T12:53:56.968+00:00"),
                DropOffDateTime = DateTimeOffset.Parse("2024-09-28T12:53:56.968+00:00"),
                PickUpLocationId = location1.Id,
                PickUpLocation = location1,
                DropOffLocationId = location1.Id,
                DropOffLocation = location1,
                Status = Status.Confirmed
            };

            await _context.Bookings.AddAsync(booking1);

            // TEST RESTRICTED DELETE BEHAVIOR. NOT WORKING IN-MEMORY DB

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
