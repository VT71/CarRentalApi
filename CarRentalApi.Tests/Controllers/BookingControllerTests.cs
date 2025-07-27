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
            UserId = "1",
            PickUpLocation = pickupLocation,
            DropOffLocation = dropoffLocation,
            Status = BookingStatus.Confirmed,
        };
    }

    [Fact]
    public async Task GetBookings_WhenNoBookingsExist_ReturnsEmptyList()
    {
        // Arrange
        _serviceMock.GetAll().Returns(new List<Booking>());

        // Act
        var controller = new BookingController(_serviceMock);
        var result = await controller.GetBookings();
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var bookings = Assert.IsAssignableFrom<IEnumerable<Booking>>(okResult.Value);

        // Assert
        Assert.NotNull(bookings);
        Assert.Empty(bookings);
    }

    [Fact]
    public async Task GetBookings_WhenBookingsExist_ReturnsBookings()
    {
        // Arrange
        var booking = GetBooking(1);
        var existingBookingList = new List<Booking> { booking };
        _serviceMock.GetAll().Returns(existingBookingList);

        // Act
        var controller = new BookingController(_serviceMock);
        var result = await controller.GetBookings();
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var bookings = Assert.IsAssignableFrom<IEnumerable<Booking>>(okResult.Value);

        // Assert
        Assert.NotNull(bookings);
        foreach (var b in bookings)
        {
            Assert.Equal(booking.Id, b.Id);
        }
    }

    [Fact]
    public async Task GetBooking_WhenBookingExists_ReturnsBooking()
    {
        // Arrange
        var booking = GetBooking(1);
        _serviceMock.GetById(1).Returns(booking);

        // Act
        var controller = new BookingController(_serviceMock);
        var result = await controller.GetBooking(1);
        var returnedBooking = result.Value;

        // Assert
        Assert.IsType<ActionResult<Booking>>(result);
        Assert.NotNull(returnedBooking);
        Assert.Equal(booking.Id, returnedBooking.Id);
    }

    [Fact]
    public async Task GetBooking_WhenBookingDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        _serviceMock.GetById(1).Returns((Booking?)null);

        // Act
        var controller = new BookingController(_serviceMock);
        var result = await controller.GetBooking(1);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task PutBooking_WhenBookingExists_UpdatesBooking()
    {
        // Arrange
        var booking = GetBooking(1);
        _serviceMock.Update(1, booking).Returns(true);

        // Act
        var controller = new BookingController(_serviceMock);
        var result = await controller.PutBooking(1, booking);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task PutBooking_WhenBookingDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var booking = GetBooking(1);
        _serviceMock.Update(1, booking).Returns(false);

        // Act
        var controller = new BookingController(_serviceMock);
        var result = await controller.PutBooking(1, booking);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task PostBooking_WhenBookingIsValid_CreatesBooking()
    {
        // Arrange
        var booking = GetBooking(1);
        _serviceMock.Create(booking).Returns(booking);

        // Act
        var controller = new BookingController(_serviceMock);
        var result = await controller.PostBooking(booking);
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var createdBooking = Assert.IsAssignableFrom<Booking>(createdResult.Value);

        // Assert
        Assert.NotNull(createdBooking);
        Assert.Equal(booking.Id, createdBooking.Id);
    }

    [Fact]
    public async Task PostBooking_WhenBookingIsInvalid_ReturnsBadRequest()
    {
        // Arrange
        Booking? booking = null;

        // Act
        var controller = new BookingController(_serviceMock);
        var result = await controller.PostBooking(booking);

        // Assert
        Assert.IsType<BadRequestResult>(result.Result);
    }

    [Fact]
    public async Task DeleteBooking_WhenBookingExists_DeletesBooking()
    {
        // Arrange
        var booking = GetBooking(1);
        _serviceMock.GetById(1).Returns(booking);
        _serviceMock.Delete(booking).Returns(Task.CompletedTask);

        // Act
        var controller = new BookingController(_serviceMock);
        var result = await controller.DeleteBooking(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteBooking_WhenBookingDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        _serviceMock.GetById(1).Returns((Booking?)null);

        // Act
        var controller = new BookingController(_serviceMock);
        var result = await controller.DeleteBooking(1);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}