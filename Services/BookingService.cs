using CarRentalApi.Data;
using CarRentalApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CarRentalApi.Services;

public class BookingService
{
    private readonly CarRentalContext _context;

    public BookingService(CarRentalContext context)
    {
        _context = context;
    }

    public async Task<Booking?> CreateBooking(Booking newBooking)
    {
        Booking? validatedBooking = ValidateBooking(newBooking);

        if (validatedBooking != null)
        {
            await _context.AddAsync(validatedBooking);
            await _context.SaveChangesAsync();
            return validatedBooking;
        }

        return null;
    }

    private Booking? ValidateBooking(Booking booking)
    {
        var car = _context.Cars.Find(booking.CarId);
        if (car != null && car.Available == true && !BookingOverlap(booking))
        {
            booking.Car = car;
        }
        else
        {
            return null;
        }


        var nowTime = DateTimeOffset.UtcNow;
        if (DateTimeOffset.Compare(booking.PickUpDateTime, nowTime) <= 0)
        {
            return null;
        }

        if (DateTimeOffset.Compare(booking.DropOffDateTime, booking.PickUpDateTime) <= 0)
        {
            return null;
        }

        var pickUpLocation = _context.Locations.Find(booking.PickUpLocationId);
        if (pickUpLocation != null && pickUpLocation.PickUpAvailable == true)
        {
            booking.PickUpLocation = pickUpLocation;

        }
        else
        {
            return null;
        }

        var dropOffLocation = _context.Locations.Find(booking.DropOffLocationId);
        if (dropOffLocation != null && dropOffLocation.DropOffAvailable == true)
        {
            booking.DropOffLocation = dropOffLocation;
        }
        else
        {
            return null;
        }

        booking.Status = Status.Pending;

        return booking;
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