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
        modelBuilder.Entity<Car>().HasOne(c => c.Make).WithMany(m => m.Cars).HasForeignKey(c => c.MakeIf).OnDelete(DeleteBehavior.Restrict).IsRequired();

        modelBuilder.Entity<Booking>()
            .HasOne(b => b.PickUpLocation)
            .WithMany(l => l.PickUpBookings)
            .HasForeignKey(b => b.PickUpLocationId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        modelBuilder.Entity<Booking>()
            .HasOne(b => b.DropOffLocation)
            .WithMany(l => l.DropOffBookings)
            .HasForeignKey(b => b.DropOffLocationId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
    }

    public DbSet<Car> Cars { get; set; }
    public DbSet<Make> Makes { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Location> Locations { get; set; }

}