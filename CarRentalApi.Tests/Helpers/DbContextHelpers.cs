using CarRentalApi.Data;
using CarRentalApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace CarRentalApi.Tests.Helpers;

public static class DbContextHelpers
{
    private static SqliteConnection _connection = new SqliteConnection("Filename=:memory:");

    private static DbContextOptions<CarRentalContext> _contextOptions = new DbContextOptionsBuilder<CarRentalContext>()
        .UseSqlite(_connection)
        .Options;

    public static CarRentalContext GetInMemoryDbContext()
    {
        return new CarRentalContext(_contextOptions);
    }

    public static void InitializeDatabase(CarRentalContext context, bool seedData = true)
    {
        context.Database.OpenConnection();
        context.Database.EnsureCreated();

        if (seedData)
        {
            // Seed data
            var passwordHasher = new PasswordHasher<IdentityUser>();
            
            var adminUser = new IdentityUser
            {
                UserName = "admin@example.com",
                Email = "admin@example.com",
            };
            adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "Admin@123");
            context.Add(adminUser);

            var employeeUser = new IdentityUser
            {
                UserName = "employee@example.com",
                Email = "Employee@123",
            };
            employeeUser.PasswordHash = passwordHasher.HashPassword(employeeUser, "Admin@123");
            context.Add(employeeUser);

            var customerUser = new IdentityUser
            {
                UserName = "customer@example.com",
                Email = "Customer@123",
            };
            customerUser.PasswordHash = passwordHasher.HashPassword(customerUser, "Admin@123");
            context.Add(customerUser);


            var make1 = new Make { Name = "Jeep" };
            var make2 = new Make { Name = "Toyota" };
            var make3 = new Make { Name = "Volvo" };

            context.Makes.Add(make1);
            context.Makes.Add(make2);
            context.Makes.Add(make3);

            var car1 = new Car
            {
                MakeId = make1.Id,
                Make = make1,
                Model = "Wrangler",
                Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.",
                PictureUrl = "https://drive.google.com/thumbnail?id=1pXzXrCSDQZgwikd41iXOx1snt1CEVmqx",
                DayPrice = 80m,
                Deposit = 250m,
                Seats = 5,
                Doors = 4,
                TransmissionType = TransmissionType.Auto,

                PowerHp = 280,
                RangeKm = 650,
                Available = true,
            };

            var car2 = new Car
            {
                MakeId = make2.Id,
                Make = make2,
                Model = "Prado",
                Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.",
                PictureUrl = "https://drive.google.com/thumbnail?id=1H36_Wo7_16oo0etKGxKN6p9nRwMHtNx9",
                DayPrice = 80m,
                Deposit = 250m,
                Seats = 5,
                Doors = 4,
                TransmissionType = TransmissionType.Auto,

                PowerHp = 320,
                RangeKm = 600,
                Available = true,
            };

            var car3 = new Car
            {
                MakeId = make3.Id,
                Make = make3,
                Model = "XC60",
                Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.",
                PictureUrl = "https://drive.google.com/thumbnail?id=18_jWUCVU6YMBDG40o3Zkb3JlBnNRyTm7",
                DayPrice = 80m,
                Deposit = 250m,
                Seats = 5,
                Doors = 4,
                TransmissionType = TransmissionType.Auto,

                PowerHp = 180,
                RangeKm = 650,
                Available = true,
            };

            context.Cars.Add(car1);
            context.Cars.Add(car2);
            context.Cars.Add(car3);

            var location1 = new Location
            {
                Name = "Central London Office",
                PickUpAvailable = true,
                DropOffAvailable = true
            };

            var location2 = new Location
            {
                Name = "Heathrow Airport",
                PickUpAvailable = true,
                DropOffAvailable = true
            };

            context.Locations.Add(location1);
            context.Locations.Add(location2);

            var booking1 = new Booking
            {
                UserId = customerUser.Id,
                CarId = car1.Id,
                Car = car1,
                PickUpDateTime = DateTimeOffset.Parse("2024-10-22T12:53:56.968+00:00"),
                DropOffDateTime = DateTimeOffset.Parse("2024-10-28T12:53:56.968+00:00"),
                PickUpLocationId = location1.Id,
                PickUpLocation = location1,
                DropOffLocationId = location1.Id,
                DropOffLocation = location1,
                Status = BookingStatus.Confirmed
            };

            var booking2 = new Booking
            {
                UserId = customerUser.Id,
                CarId = car2.Id,
                Car = car2,
                PickUpDateTime = DateTimeOffset.Parse("2024-11-01T12:53:56.968+00:00"),
                DropOffDateTime = DateTimeOffset.Parse("2024-11-11T12:53:56.968+00:00"),
                PickUpLocationId = location1.Id,
                PickUpLocation = location1,
                DropOffLocationId = location1.Id,
                DropOffLocation = location1,
                Status = BookingStatus.Confirmed
            };

            var booking3 = new Booking
            {
                UserId = customerUser.Id,
                CarId = car3.Id,
                Car = car3,
                PickUpDateTime = DateTimeOffset.Parse("2024-11-13T12:53:56.968+00:00"),
                DropOffDateTime = DateTimeOffset.Parse("2024-11-24T12:53:56.968+00:00"),
                PickUpLocationId = location1.Id,
                PickUpLocation = location1,
                DropOffLocationId = location1.Id,
                DropOffLocation = location1,
                Status = BookingStatus.Confirmed
            };

            context.Bookings.Add(booking1);
            context.Bookings.Add(booking2);
            context.Bookings.Add(booking3);

            context.SaveChanges();
        }
    }

    public static void CloseDatabase(CarRentalContext context)
    {
        context.Database.CloseConnection();
    }
}