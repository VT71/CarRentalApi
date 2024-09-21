using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarRentalApi.Data;
using CarRentalApi.Models;

namespace CareRentalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly CarRentalContext _context;

        public BookingController(CarRentalContext context)
        {
            _context = context;
        }

        // GET: api/Booking
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookings()
        {
            return await _context.Bookings.ToListAsync();
        }

        // GET: api/Booking/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Booking>> GetBooking(long id)
        {
            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null)
            {
                return NotFound();
            }

            return booking;
        }

        // PUT: api/Booking/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBooking(long id, Booking booking)
        {
            if (id != booking.Id)
            {
                return BadRequest();
            }

            _context.Entry(booking).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookingExists(id))
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

        // POST: api/Booking
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Booking>> PostBooking(Booking booking)
        {
            var car = await _context.Cars.FindAsync(booking.CarId);
            if (car != null && car.Available == true && !BookingOverlap(booking))
            {
                booking.Car = car;
            }
            else
            {
                ModelState.AddModelError(nameof(Booking.Car), "Invalid Car");
            }


            var nowTime = DateTimeOffset.UtcNow;
            if (DateTimeOffset.Compare(booking.PickUpDateTime, nowTime) <= 0)
            {
                ModelState.AddModelError(nameof(Booking.PickUpDateTime), "Invalid Pick Up Date");
            }

            if (DateTimeOffset.Compare(booking.DropOffDateTime, booking.PickUpDateTime) <= 0)
            {
                ModelState.AddModelError(nameof(Booking.DropOffDateTime), "Invalid Drop Off Date");
            }

            var pickUpLocation = await _context.Locations.FindAsync(booking.PickUpLocationId);
            if (pickUpLocation != null && pickUpLocation.PickUpAvailable == true)
            {
                booking.PickUpLocation = pickUpLocation;

            }
            else
            {
                ModelState.AddModelError(nameof(Booking.PickUpLocation), "Invalid Pick Up Location");
            }

            var dropOffLocation = await _context.Locations.FindAsync(booking.DropOffLocationId);
            if (dropOffLocation != null && dropOffLocation.DropOffAvailable == true)
            {
                booking.DropOffLocation = dropOffLocation;
            }
            else
            {
                ModelState.AddModelError(nameof(Booking.DropOffLocation), "Invalid Drop Off Location");
            }

            if (!ValidStatus(booking.Status))
            {
                ModelState.AddModelError(nameof(Booking.Status), "Invalid Status");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, booking);
        }

        // DELETE: api/Booking/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(long id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookingExists(long id)
        {
            return _context.Bookings.Any(e => e.Id == id);
        }

        private bool ValidStatus(object? value)
        {
            if (value != null && Enum.IsDefined(typeof(Status), value))
            {
                return true;
            }
            return false;
        }

        private bool BookingOverlap(Booking newBooking)
        {
            return _context.Bookings.Any(b => b.CarId == newBooking.CarId && DateTimeOffset.Compare(newBooking.PickUpDateTime, b.DropOffDateTime) < 0 && DateTimeOffset.Compare(newBooking.DropOffDateTime, b.PickUpDateTime) > 0);
        }
    }
}
