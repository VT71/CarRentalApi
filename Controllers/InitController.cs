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
            var make3 = new Make { Name = "Volvo" };

            await _context.Makes.AddAsync(make1);
            await _context.Makes.AddAsync(make2);
            await _context.Makes.AddAsync(make3);

            var car1 = new Car
            {
                MakeId = make1.Id,
                Make = make1,
                Model = "Wrangler",
                Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.",
                PictureUrl = "https://drive.google.com/thumbnail?id=1pXzXrCSDQZgwikd41iXOx1snt1CEVmqx",
                DayPrice = 80m,
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
                Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.",
                PictureUrl = "https://drive.google.com/thumbnail?id=1H36_Wo7_16oo0etKGxKN6p9nRwMHtNx9",
                DayPrice = 80m,
                Deposit = 250m,
                Seats = 5,
                Doors = 4,
                TransmissionType = TransmissionType.Auto,

                PowerHp = 320,
                RangeKm = 600,
                Available = true,
            };

            var car3 = new Car
            {
                MakeId = make3.Id,
                Make = make3,
                Model = "XC60",
                Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.",
                PictureUrl = "https://drive.google.com/thumbnail?id=18_jWUCVU6YMBDG40o3Zkb3JlBnNRyTm7",
                DayPrice = 80m,
                Deposit = 250m,
                Seats = 5,
                Doors = 4,
                TransmissionType = TransmissionType.Auto,

                PowerHp = 180,
                RangeKm = 650,
                Available = true,
            };

            await _context.Cars.AddAsync(car1);
            await _context.Cars.AddAsync(car2);
            await _context.Cars.AddAsync(car3);

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
