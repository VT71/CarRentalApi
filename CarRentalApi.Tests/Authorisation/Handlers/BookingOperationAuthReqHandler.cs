using CarRentalApi.Authorisation;
using CarRentalApi.Authorisation.Handlers;
using CarRentalApi.Authorisation.Requirements;
using CarRentalApi.Models;
using CarRentalApi.Tests.Helpers;
using Humanizer;
using Microsoft.AspNetCore.Authorization;

namespace CarRentalApi.Tests.Authorisation.Handlers;

public class BookingOperationAuthReqHandlerTests
{
    private BookingOperationAuthReqHandler _handler;
    public BookingOperationAuthReqHandlerTests()
    {
        _handler = new BookingOperationAuthReqHandler();
    }
    
    [Theory]
    [InlineData(Roles.Admin)]
    [InlineData(Roles.Employee)]
    public async Task HandleRequirementAsyncRead_IfAdminOrEmployee_Succeed(string role)
    {
        // Arrange
        var resource = new PaginatedList<Booking>(new List<Booking>(), 0, 1, 10);
        var authContext = new AuthorizationHandlerContext(
            new List<IAuthorizationRequirement>
            {
                Operations.Read
            },
            UserHelpers.CreateUserWithRoles("u1", [ role ]),
            resource
        );
        // Act
        await _handler.HandleAsync(authContext);

        // Assert
        Assert.True(authContext.HasSucceeded);
    }

    [Theory]
    [InlineData("empty")]
    [InlineData("partial-ownership")]
    [InlineData("full-ownership")]
    public async Task HandleRequirementAsyncRead_IfEmployee_SucceedBasedOnOwnership(string caseType)
    {
        // Arrange
        var userId = "u1";
        List<Booking> bookings;
        switch (caseType)
        {
            case "empty":
                bookings = new List<Booking>();
                break;
            case "partial-ownership":
                bookings = new List<Booking>
                {
                    BookingHelpers.GetBooking(1, userId),
                    BookingHelpers.GetBooking(2, "different-user")
                };
                break;
            case "full-ownership":
                bookings = new List<Booking>
                {
                    BookingHelpers.GetBooking(1, userId),
                    BookingHelpers.GetBooking(2, userId)
                };
                break;
            default:
                throw new ArgumentException("Invalid case type");
        }
        var resource = new PaginatedList<Booking>(bookings, bookings.Count, 1, 10);
        var authContext = new AuthorizationHandlerContext(
            new List<IAuthorizationRequirement>
            {
                Operations.Read
            },
            UserHelpers.CreateUserWithRoles(userId, [ Roles.Customer ]),
            resource
        );

        // Act
        await _handler.HandleAsync(authContext);

        // Assert
        if (caseType == "full-ownership" || caseType == "empty")
        {
            Assert.True(authContext.HasSucceeded);
        }
        else
        {
            Assert.False(authContext.HasSucceeded);
        }
    }
}