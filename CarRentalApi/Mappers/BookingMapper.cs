using CarRentalApi.Interfaces;
using CarRentalApi.Models;

namespace CarRentalApi.Mappers;

public class BookingMapper : IBookingMapper
{
    public Booking ToModel(Booking booking, BookingDto dto)
    {
        return new Booking {
            Id = booking.Id,
            UserId = booking.UserId,
            CarId = booking.CarId,
            Car = booking.Car,
            PickUpDateTime = booking.PickUpDateTime,
            DropOffDateTime = booking.DropOffDateTime,
            PickUpLocationId = booking.PickUpLocationId,
            PickUpLocation = booking.PickUpLocation,
            DropOffLocationId = booking.DropOffLocationId,
            DropOffLocation = booking.DropOffLocation,
            Status = booking.Status
        };
        // DEFAULT: TO CHANGE
    }
    public BookingDto ToDto(Booking booking)
    {
        return new BookingDto();
        // DEFAULT: TO CHANGE
    }
    public Booking ToModel(Booking booking, BookingPatchDto patchDto)
    {
        booking.Status = patchDto.Status;
        return booking;
    }
}