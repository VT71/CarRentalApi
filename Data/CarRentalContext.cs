using Microsoft.EntityFrameworkCore;
using CarRentalApi.Models;

namespace CarRentalApi.Data;

public class CarRentalContext : DbContext
{
    public CarRentalContext(DbContextOptions<CarRentalContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {


        modelBuilder.Entity<Booking>()
            .HasOne(b => b.PickUpLocation)
            .WithMany(l => l.PickUpBookings)
            .HasForeignKey(b => b.PickUpLocationId)
            .IsRequired().OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Booking>()
            .HasOne(b => b.DropOffLocation)
            .WithMany(l => l.DropOffBookings)
            .HasForeignKey(b => b.DropOffLocationId)
            .IsRequired().OnDelete(DeleteBehavior.Restrict);
    }

    public DbSet<Car> Cars { get; set; }
    public DbSet<Make> Makes { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Location> Locations { get; set; }

}