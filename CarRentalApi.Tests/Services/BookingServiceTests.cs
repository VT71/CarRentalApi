using CarRentalApi.Data;
using CarRentalApi.Models;
using CarRentalApi.Services;
using CarRentalApi.Tests.Helpers;
using Microsoft.EntityFrameworkCore;

namespace CarRentalApi.Tests.Services;

public class BookingServiceTests
{
    private CarRentalContext context;

    public BookingServiceTests()
    {
        context = DbContextHelpers.GetInMemoryDbContext();
    }

    [Theory]
    [InlineData(true, 1, 10)]
    [InlineData(true, 2, 5)]
    [InlineData(false, 2, 5)]
    public async Task GetAllAsync_WhenBookingsExist_ReturnsBookings(bool bookingsExist, int pageIndex, int pageSize)
    {
        // Arrange 
        DbContextHelpers.InitializeDatabase(context);
        if (!bookingsExist)
            context.Bookings.ExecuteDelete();

        var query = new PaginatedQuery
        {
            PageIndex = pageIndex,
            PageSize = pageSize
        };
        var allBookings = context.Bookings;
        var expectedBookings = allBookings.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

        // Act
        var service = new BookingService(context);
        var result = await service.GetAllAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<PaginatedList<Booking>>(result);
        Assert.Equal(pageIndex, result.PageIndex);
        Assert.Equal(pageSize, result.PageSize);
        Assert.Equal(allBookings.Count(), result.TotalCount);
        Assert.Equal(expectedBookings.Count(), result.Items.Count());
        foreach (var b in result.Items)
        {
            var exists = expectedBookings.Exists((expectedBooking) => expectedBooking.Id == b.Id);
            Assert.True(exists);
        }

        DbContextHelpers.CloseDatabase(context);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetByCustomerIdAsync_ReturnsBookingsOrEmpty(bool bookingsExist)
    {
        // Arrange 
        DbContextHelpers.InitializeDatabase(context);
        if (!bookingsExist)
            context.Bookings.ExecuteDelete();

        var allUsers = context.Users.ToList();
        var customerId = context.Users.Where((u) => u.Email == "customer@example.com").Select((u) => u.Id).FirstOrDefault();
        var query = new PaginatedQuery
        {
            PageIndex = 1,
            PageSize = 10
        };
        var allBookings = context.Bookings.Where((b) => b.UserId == customerId).ToList();
        var expectedBookings = allBookings.Skip((1 - 1) * 10).Take(10).ToList();

        // Act
        var service = new BookingService(context);
        var result = await service.GetAllAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<PaginatedList<Booking>>(result);
        Assert.Equal(1, result.PageIndex);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(allBookings.Count(), result.TotalCount);
        Assert.Equal(expectedBookings.Count(), result.Items.Count());
        foreach (var b in result.Items)
        {
            var exists = expectedBookings.Exists((expectedBooking) => expectedBooking.Id == b.Id);
            Assert.True(exists);
        }

        DbContextHelpers.CloseDatabase(context);
    }
}