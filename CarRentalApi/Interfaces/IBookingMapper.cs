using CarRentalApi.Models;

namespace CarRentalApi.Interfaces;

public interface IBookingMapper
{
    BookingDto ToDto(Booking booking);
    Booking ToModel(Booking booking, BookingDto dto);
    Booking ToModel(Booking booking, BookingPatchDto patchDto);
}