using System.Threading.Tasks;
using CareRentalApi.Controllers;
using CarRentalApi.Models;
using CarRentalApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace CarRentalApi.Tests.Controllers;

public class BookingControllerTests
{
    private IBookingService _serviceMock;

    public BookingControllerTests()
    {
        _serviceMock = Substitute.For<IBookingService>();
    }

    private Booking GetBooking(long id)
    {
        var make = new Make { Id = 1, Name = "Test Make" };

        var car = new Car {
            Id = 1, Model = "Test Car",
            Make = make, Description = "Test Description",
            PictureUrl = "http://example.com/car.jpg", 
            TransmissionType = TransmissionType.Auto };

        var pickupLocation = new Location { Id = 1, Name = "Test Pickup" };
        var dropoffLocation = new Location { Id = 2, Name = "Test Drop"};
    
        return new Booking
        {
            Id = id,
            CarId = car.Id,
            Car = car,
            UserId = "1",
            PickUpLocation = pickupLocation,
            DropOffLocation = dropoffLocation,
            Status = BookingStatus.Confirmed,
        };
    }

    public async Task GetAllBookings_WhenNoBookingsExist_ReturnsEmptyList()
    {
        // Arrange
        _serviceMock.GetAll().Returns(new List<Booking>());

        // Act
        var controller = new BookingController(_serviceMock);
        var result = await controller.GetBookings();
        var bookings = result.Value;

        // Assert
        Assert.IsType<ActionResult<IEnumerable<Booking>>>(result);
        Assert.IsType<OkObjectResult>(result.Result);
        Assert.NotNull(bookings);
        Assert.Empty(bookings);
    }

    public async Task GetBooking_WhenBookingExists_ReturnsBooking()
    {
        // Arrange
        var booking = GetBooking(1);
        _serviceMock.GetAll().Returns(new List<Booking> { booking });

        // Act
        var controller = new BookingController(_serviceMock);
        var result = await controller.GetBooking(1);
        var returnedBooking = result.Value;

        // Assert
        Assert.IsType<ActionResult<Booking>>(result);
        Assert.NotNull(returnedBooking);
        Assert.Equal(booking.Id, returnedBooking.Id);
    }
}