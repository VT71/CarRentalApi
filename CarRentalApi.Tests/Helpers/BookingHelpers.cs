using CarRentalApi.Models;

namespace CarRentalApi.Tests.Helpers;
public static class BookingHelpers
{
    public static Booking GetBooking(long id, string userId = "1")
    {
        var make = new Make { Id = 1, Name = "Test Make" };

        var car = new Car
        {
            Id = 1,
            Model = "Test Car",
            Make = make,
            Description = "Test Description",
            PictureUrl = "http://example.com/car.jpg",
            TransmissionType = TransmissionType.Auto
        };

        var pickupLocation = new Location { Id = 1, Name = "Test Pickup" };
        var dropoffLocation = new Location { Id = 2, Name = "Test Drop" };

        return new Booking
        {
            Id = id,
            CarId = car.Id,
            Car = car,
            UserId = userId,
            PickUpLocation = pickupLocation,
            DropOffLocation = dropoffLocation,
            Status = BookingStatus.Confirmed,
        };
    }
}