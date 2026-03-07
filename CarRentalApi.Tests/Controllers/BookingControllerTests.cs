using System.Security.Claims;
using CarRentalApi.Authorisation;
using CarRentalApi.Authorisation.Requirements;
using CarRentalApi.Controllers;
using CarRentalApi.Models;
using CarRentalApi.Services.Interfaces;
using CarRentalApi.Tests.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace CarRentalApi.Tests.Controllers;

public class BookingControllerTests
{
    private IBookingService _serviceMock;
    private IAuthorizationService _authorizationServiceMock;

    public BookingControllerTests()
    {
        _serviceMock = Substitute.For<IBookingService>();
        _authorizationServiceMock = Substitute.For<IAuthorizationService>();
    }

    [Fact]
    public void Controller_RequiresAuthorizationAttribute()
    {
        // Arrange
        var controllerType = typeof(BookingController);

        // Act
        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), true);

        // Assert
        Assert.NotEmpty(authorizeAttribute);
    }

    [Fact]
    public async Task GetBookings_RequireAdminOrEmployeeRole()
    {
        // Arrange
        var methodInfo = typeof(BookingController).GetMethod("GetBookings");
        var authorizeAttribute = methodInfo?.GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), true)
            .Cast<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>()
            .FirstOrDefault();

        // Assert
        Assert.NotNull(authorizeAttribute);
        Assert.Equal($"{Roles.Admin},{Roles.Employee}", authorizeAttribute.Roles);
    }

    [Fact]
    public async Task GetBookings_WhenNoBookingsExist_ReturnsEmptyList()
    {
        // Arrange
        _serviceMock.GetAllAsync(Arg.Any<PaginatedQuery>()).Returns(new PaginatedList<Booking>(new List<Booking>(), 0, 1, 10));

        // Act
        var controller = new BookingController(_serviceMock, _authorizationServiceMock);
        var result = await controller.GetBookings(new PaginatedQuery());

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var paginatedList = Assert.IsAssignableFrom<PaginatedList<Booking>>(okResult.Value);
        Assert.NotNull(paginatedList);
        Assert.Empty(paginatedList.Items);
    }

    [Fact]
    public async Task GetBookings_WhenBookingsExist_ReturnsBookings()
    {
        // Arrange
        var booking = BookingHelpers.GetBooking(1);
        var paginatedList = new PaginatedList<Booking>(new List<Booking> { booking }, 1, 1, 1);
        _serviceMock.GetAllAsync(Arg.Any<PaginatedQuery>()).Returns(paginatedList);

        // Act
        var controller = new BookingController(_serviceMock, _authorizationServiceMock);
        var result = await controller.GetBookings(new PaginatedQuery());
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var bookings = Assert.IsAssignableFrom<PaginatedList<Booking>>(okResult.Value);

        // Assert
        Assert.NotNull(bookings);
        foreach (var b in bookings.Items)
        {
            Assert.Equal(booking.Id, b.Id);
        }
    }

    [Fact]
    public async Task GetBooking_RequireAdminEmployeeCustomerRole()
    {
        // Arrange
        var methodInfo = typeof(BookingController).GetMethod("GetBooking");
        var authorizeAttribute = methodInfo?.GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), true)
            .Cast<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>()
            .FirstOrDefault();

        // Assert
        Assert.NotNull(authorizeAttribute);
        Assert.Equal($"{Roles.Admin}, {Roles.Employee}, {Roles.Customer}", authorizeAttribute.Roles);
    }

    [Fact]
    public async Task GetBooking_WhenBookingExists_ReturnsBooking()
    {
        // Arrange
        var booking = BookingHelpers.GetBooking(1);
        _serviceMock.GetById(1).Returns(booking);
        _authorizationServiceMock.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<PaginatedList<Booking>>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>()).Returns(Task.FromResult(AuthorizationResult.Success()));

        // Act
        var controller = new BookingController(_serviceMock, _authorizationServiceMock);
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
        _authorizationServiceMock.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<PaginatedList<Booking>>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>()).Returns(Task.FromResult(AuthorizationResult.Success()));

        // Act
        var controller = new BookingController(_serviceMock, _authorizationServiceMock);
        var result = await controller.GetBooking(1);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetBoooking_WhenNotAuthorised_ReturnsForbid()
    {
        // Arrange
        var booking = BookingHelpers.GetBooking(1);
        _serviceMock.GetById(1).Returns(booking);
        _authorizationServiceMock.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<PaginatedList<Booking>>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>()).Returns(Task.FromResult(AuthorizationResult.Failed()));

        // Act
        var controller = new BookingController(_serviceMock, _authorizationServiceMock);
        var result = await controller.GetBooking(1);

        // Assert
        Assert.IsType<ForbidResult>(result.Result);
    }

    [Fact]
    public async Task GetBookingsByCustomer_RequireAdminEmployeeOrCustomerRole()
    {
        // Arrange
        var methodInfo = typeof(BookingController).GetMethod("GetBookingsByCustomer");
        var authorizeAttribute = methodInfo?.GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), true)
            .Cast<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>()
            .FirstOrDefault();

        // Assert
        Assert.NotNull(authorizeAttribute);
        Assert.Equal($"{Roles.Admin},{Roles.Employee},{Roles.Customer}", authorizeAttribute.Roles);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task GetBookingsByCustomer_WhenAndAuthorised_ReturnsBookings(bool bookingsExist)
    {
        // Arrange
        var customerId = "1";
        var booking = BookingHelpers.GetBooking(1);
        var paginatedList = bookingsExist
            ? new PaginatedList<Booking>(new List<Booking> { booking }, 1, 1, 1)
            : new PaginatedList<Booking>(new List<Booking>(), 0, 1, 1);
        _serviceMock.GetByCustomerIdAsync(customerId, Arg.Any<PaginatedQuery>()).Returns(paginatedList);
        _authorizationServiceMock.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<PaginatedList<Booking>>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>()).Returns(Task.FromResult(AuthorizationResult.Success()));

        // Act
        var controller = new BookingController(_serviceMock, _authorizationServiceMock);
        var result = await controller.GetBookingsByCustomer(customerId, new PaginatedQuery());

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var bookingsResult = Assert.IsAssignableFrom<PaginatedList<Booking>>(okResult.Value);
        if (bookingsExist)
        {
            Assert.Equal(booking.Id, bookingsResult.Items.First().Id);
            Assert.Equal(customerId, bookingsResult.Items.First().UserId);
        }
        else
        {
            Assert.Empty(bookingsResult.Items);
        }
    }

    [Fact]
    public async Task GetBookingsByCustomer_WhenForbid_ReturnsError()
    {
        // Arrange
        _serviceMock.GetByCustomerIdAsync(Arg.Any<string>(), Arg.Any<PaginatedQuery>()).Returns(new PaginatedList<Booking>(new List<Booking>(), 0, 1, 1));
        _authorizationServiceMock.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<PaginatedList<Booking>>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>()).Returns(Task.FromResult(AuthorizationResult.Failed()));

        // Act
        var controller = new BookingController(_serviceMock, _authorizationServiceMock);
        var result = await controller.GetBookingsByCustomer("1", new PaginatedQuery());

        // Assert
        Assert.IsAssignableFrom<ForbidResult>(result.Result);
    }

    [Fact]
    public async Task PutBooking_RequiresAdminEmployeeRoles()
    {
        // Arrange
        var methodInfo = typeof(BookingController).GetMethod("PutBooking");
        var authorizeAttribute = methodInfo?.GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), true)
            .Cast<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>()
            .FirstOrDefault();

        // Assert
        Assert.NotNull(authorizeAttribute);
        Assert.Equal($"{Roles.Admin}, {Roles.Employee}", authorizeAttribute.Roles);
    }

    [Fact]
    public async Task PutBooking_WhenBookingExists_UpdatesBooking()
    {
        // Arrange
        var booking = BookingHelpers.GetBooking(1);
        _serviceMock.Update(1, booking).Returns(true);
        _authorizationServiceMock.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<PaginatedList<Booking>>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>()).Returns(Task.FromResult(AuthorizationResult.Success()));

        // Act
        var controller = new BookingController(_serviceMock, _authorizationServiceMock);
        var result = await controller.PutBooking(1, booking);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task PutBooking_WhenBookingDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var booking = BookingHelpers.GetBooking(1);
        _serviceMock.Update(1, booking).Returns(false);
        _authorizationServiceMock.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<PaginatedList<Booking>>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>()).Returns(Task.FromResult(AuthorizationResult.Success()));

        // Act
        var controller = new BookingController(_serviceMock, _authorizationServiceMock);
        var result = await controller.PutBooking(1, booking);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task PutBooking_WhenNotAuthorised_ReturnsForbid()
    {
        // Arrange
        var booking = BookingHelpers.GetBooking(1);
        _serviceMock.Update(1, booking).Returns(true);
        _authorizationServiceMock.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<PaginatedList<Booking>>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>()).Returns(Task.FromResult(AuthorizationResult.Failed()));

        // Act
        var controller = new BookingController(_serviceMock, _authorizationServiceMock);
        var result = await controller.PutBooking(1, booking);

        // Assert
        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task PostBooking_RequiresAdminEmployeeOrCustomerRoles()
    {
        // Arrange
        var methodInfo = typeof(BookingController).GetMethod("PostBooking");
        var authorizeAttribute = methodInfo?.GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), true)
            .Cast<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>()
            .FirstOrDefault();

        // Assert
        Assert.NotNull(authorizeAttribute);
        Assert.Equal($"{Roles.Admin}, {Roles.Employee}, {Roles.Customer}", authorizeAttribute.Roles);
    }

    [Fact]
    public async Task PostBooking_WhenBookingIsValid_CreatesBooking()
    {
        // Arrange
        var booking = BookingHelpers.GetBooking(1);
        _serviceMock.Create(booking).Returns(booking);
        _authorizationServiceMock.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<PaginatedList<Booking>>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>()).Returns(Task.FromResult(AuthorizationResult.Success()));

        // Act
        var controller = new BookingController(_serviceMock, _authorizationServiceMock);
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
        Booking booking = BookingHelpers.GetBooking(1);
        _authorizationServiceMock.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<PaginatedList<Booking>>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>()).Returns(Task.FromResult(AuthorizationResult.Success()));
        _serviceMock.Create(Arg.Any<Booking>()).Returns((Booking?)null);

        // Act
        var controller = new BookingController(_serviceMock, _authorizationServiceMock);
        var result = await controller.PostBooking(booking);

        // Assert
        Assert.IsType<BadRequestResult>(result.Result);
    }

    [Fact]
    public async Task PostBooking_WhenNotAuthorised_ReturnsForbid()
    {
        // Arrange
        var booking = BookingHelpers.GetBooking(1);
        _authorizationServiceMock.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<PaginatedList<Booking>>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>()).Returns(Task.FromResult(AuthorizationResult.Failed()));

        // Act
        var controller = new BookingController(_serviceMock, _authorizationServiceMock);
        var result = await controller.PostBooking(booking);

        // Assert
        Assert.IsType<ForbidResult>(result.Result);
    }

    [Fact]
    public async Task PatchBooking_RequiresAdminEmployeeRoles()
    {
        // Arrange
        var methodInfo = typeof(BookingController).GetMethod("PatchBooking");
        var authorizeAttribute = methodInfo?.GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), true)
            .Cast<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>()
            .FirstOrDefault();

        // Assert
        Assert.NotNull(authorizeAttribute);
        Assert.Equal($"{Roles.Admin}, {Roles.Employee}", authorizeAttribute.Roles);
    }

    [Fact]
    public async Task PatchBooking_InvalidId_ReturnsBadRequest()
    {
        // Arrange
        var bookingPatchDto = new BookingPatchDto
        {
            Id = 1,
            Status = BookingStatus.Cancelled
        };

        // Act
        var controller = new BookingController(_serviceMock, _authorizationServiceMock);
        var result = await controller.PatchBooking(2, bookingPatchDto);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task PatchBooking_WhenAuthorizationFails_ReturnsForbid()
    {
        // Arrange
        var bookingPatchDto = new BookingPatchDto
        {
            Id = 1,
            Status = BookingStatus.Cancelled
        };
        _authorizationServiceMock.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<PaginatedList<Booking>>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>()).Returns(Task.FromResult(AuthorizationResult.Failed()));

        // Act
        var controller = new BookingController(_serviceMock, _authorizationServiceMock);
        var result = await controller.PatchBooking(1, bookingPatchDto);

        // Assert
        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task PatchBooking_WhenBookingNotFound_ReturnsNotFound()
    {
        // Arrange
        var bookingPatchDto = new BookingPatchDto
        {
            Id = 1,
            Status = BookingStatus.Cancelled
        };
        _authorizationServiceMock.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<PaginatedList<Booking>>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>()).Returns(Task.FromResult(AuthorizationResult.Success()));
        _serviceMock.Patch(Arg.Any<BookingPatchDto>()).Returns((long?)null);

        // Act
        var controller = new BookingController(_serviceMock, _authorizationServiceMock);
        var result = await controller.PatchBooking(bookingPatchDto.Id, bookingPatchDto);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task PatchBooking_WhenBookingFound_ReturnsNoContent()
    {
        // Arrange
        var bookingPatchDto = new BookingPatchDto
        {
            Id = 1,
            Status = BookingStatus.Cancelled
        };
        _authorizationServiceMock.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<PaginatedList<Booking>>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>()).Returns(Task.FromResult(AuthorizationResult.Success()));
        _serviceMock.Patch(Arg.Any<BookingPatchDto>()).Returns(1L);

        // Act
        var controller = new BookingController(_serviceMock, _authorizationServiceMock);
        var result = await controller.PatchBooking(bookingPatchDto.Id, bookingPatchDto);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }
}