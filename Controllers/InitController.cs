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

            await _context.Makes.AddAsync(make1);
            await _context.Makes.AddAsync(make2);

            var car1 = new Car
            {
                MakeId = make1.Id,
                Make = make1,
                Model = "Wrangler",
                Description = "Some description",
                Deposit = 250m,
                Seats = 5,
                Doors = 4,
                TransmissionType = "auto",

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
                TransmissionType = "auto",

                PowerHp = 320,
                RangeKm = 600,
                Available = true,
            };

            await _context.Cars.AddAsync(car1);
            await _context.Cars.AddAsync(car2);

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
