using Microsoft.AspNetCore.Mvc;
using CarRentalApi.Models;
using CarRentalApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace CarRentalApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _service;

        public BookingController(IBookingService service)
        {
            _service = service;
        }

        // GET: api/Booking
        [HttpGet()]
        [Authorize(Roles = "Admin,Employee")]
        [ProducesResponseType(typeof(PaginatedList<Booking>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<PaginatedList<Booking>>> GetBookings([FromQuery] PaginatedQuery query)
        {
            var bookings = await _service.GetAllAsync(query);
            return Ok(bookings);
        }

        // GET: api/Booking/Customer/5
        [HttpGet("customer/{customerId}")]
        [Authorize(Roles = "Admin,Employee,Customer")]
        [ProducesResponseType(typeof(PaginatedList<Booking>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<PaginatedList<Booking>>> GetBookingsByCustomer(string customerId, [FromQuery] PaginatedQuery query)
        {
            var bookings = await _service.GetByCustomerId(customerId, query);
            return Ok(bookings);
        }
        
        // GET: api/Booking/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Booking>> GetBooking(long id)
        {
            var booking = await _service.GetById(id);

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

            bool bookingModified = await _service.Update(id, booking);

            if (!bookingModified)
            {
                return NotFound();
            }

            return NoContent();
        }

        // POST: api/Booking
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Booking>> PostBooking(Booking booking)
        {
            Booking? newBooking = await _service.Create(booking);

            if (newBooking == null)
            {
                return BadRequest();
            }

            return CreatedAtAction(nameof(GetBooking), new { id = newBooking.Id }, newBooking);
        }

        // DELETE: api/Booking/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(long id)
        {
            var booking = await _service.GetById(id);

            if (booking == null)
            {
                return NotFound();
            }

            await _service.Delete(booking);

            return NoContent();
        }

        [HttpGet("countries")]
        public string[] GetAllCountries()
        {
            return this._service.GetAllCountries();
        }
    }
}
