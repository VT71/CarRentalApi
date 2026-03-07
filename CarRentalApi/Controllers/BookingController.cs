using Microsoft.AspNetCore.Mvc;
using CarRentalApi.Models;
using CarRentalApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using CarRentalApi.Authorisation;
using CarRentalApi.Authorisation.Requirements;

namespace CarRentalApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _service;
        private readonly IAuthorizationService _authorizationService;

        public BookingController(
            IBookingService service,
            IAuthorizationService authorizationService)
        {
            _service = service;
            _authorizationService = authorizationService;
        }

        // GET: api/Booking
        [HttpGet()]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Employee}")]
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
        [Authorize(Roles = $"{Roles.Admin},{Roles.Employee},{Roles.Customer}")]
        [ProducesResponseType(typeof(PaginatedList<Booking>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<PaginatedList<Booking>>> GetBookingsByCustomer(string customerId, [FromQuery] PaginatedQuery query)
        {
            var bookings = await _service.GetByCustomerIdAsync(customerId, query);

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, bookings, Operations.Read);

            if (authorizationResult.Succeeded)
            {
                return Ok(bookings);
            }

            return Forbid();
        }

        // GET: api/Booking/5
        [HttpGet("{id}")]
        [Authorize(Roles = $"{Roles.Admin}, {Roles.Employee}, {Roles.Customer}")]
        [ProducesResponseType(typeof(Booking), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<Booking>> GetBooking(long id)
        {
            var booking = await _service.GetById(id);

            if (booking == null)
            {
                return NotFound();
            }

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, new PaginatedList<Booking>(new List<Booking> { booking }, 1, 0, 1), Operations.Read);

            if (authorizationResult.Succeeded)
            {
                return booking;
            }

            return Forbid();
        }

        // PUT: api/Booking/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = $"{Roles.Admin}, {Roles.Employee}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutBooking(long id, Booking booking)
        {
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, new PaginatedList<Booking>(new List<Booking> { booking }, 1, 0, 1), Operations.Update);

            if (authorizationResult.Succeeded)
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

            return Forbid();
        }

        // POST: api/Booking
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = $"{Roles.Admin}, {Roles.Employee}, {Roles.Customer}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<Booking>> PostBooking(Booking booking)
        {
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, new PaginatedList<Booking>(new List<Booking> { booking }, 1, 0, 1), Operations.Create);

            if (authorizationResult.Succeeded)
            {
                Booking? newBooking = await _service.Create(booking);

                if (newBooking == null)
                {
                    return BadRequest();
                }

                return CreatedAtAction(nameof(GetBooking), new { id = newBooking.Id }, newBooking);
            }

            return Forbid();
        }

        // Patch: api/Booking/:id
        [HttpPatch("{id}")]
        [Authorize(Roles = $"{Roles.Admin}, {Roles.Employee}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PatchBooking(long id, BookingPatchDto bookingPatchDto)
        {
            if (bookingPatchDto.Id != id)
            {
                return BadRequest();
            }

            var booking = await _service.GetById(id);
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, new PaginatedList<Booking>(new List<Booking> { booking }, 1, 0, 1), Operations.Update);

            if (authorizationResult.Succeeded)
            {
                var bookingId = await _service.Patch(bookingPatchDto);

                if (bookingId == null)
                {
                    return NotFound();
                }

                return NoContent();
            }

            return Forbid();
        }

        [HttpGet("countries")]
        public string[] GetAllCountries()
        {
            return this._service.GetAllCountries();
        }
    }
}
