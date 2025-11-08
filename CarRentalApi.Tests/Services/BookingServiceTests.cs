using CarRentalApi.Data;
using CarRentalApi.Models;
using CarRentalApi.Services;
using CarRentalApi.Tests.Helpers;

namespace CarRentalApi.Tests.Services;

public class BookingServiceTests
{
    private CarRentalContext context;

    public BookingServiceTests()
    {
        context = DbContextHelpers.GetInMemoryDbContext();
    }

    [Theory]
    [InlineData(1, 10)]
    [InlineData(2, 5)]
    public async Task GetAll_WhenBookingsExist_ReturnsBookings(int pageIndex, int pageSize)
    {
        // Arrange 
        DbContextHelpers.InitializeDatabase(context);

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

    [Fact]
    public async Task GetAll_WhenNoBookings_ReturnsEmptyList()
    {
        // Arrange 
        DbContextHelpers.InitializeDatabase(context, seedData: false);

        var query = new PaginatedQuery
        {
            PageIndex = 1,
            PageSize = 10
        };

        // Act
        var service = new BookingService(context);
        var result = await service.GetAllAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<PaginatedList<Booking>>(result);
        Assert.Equal(0, result.TotalCount);
        Assert.Empty(result.Items);

        DbContextHelpers.CloseDatabase(context);
    }
}