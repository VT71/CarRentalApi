using Microsoft.EntityFrameworkCore;
using CarRentalApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace CarRentalApi.Data;

public class CarRentalContext : IdentityDbContext<IdentityUser>
{
    public CarRentalContext(DbContextOptions<CarRentalContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Booking relationships
        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Car)
            .WithMany(c => c.Bookings)
            .HasForeignKey(b => b.CarId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        modelBuilder.Entity<Car>()
            .HasOne(c => c.Make)
            .WithMany(m => m.Cars)
            .HasForeignKey(c => c.MakeId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

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

        // Timestamps configuration
        modelBuilder.Entity<Booking>()
            .Property(b => b.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()")
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<Booking>()
            .Property(b => b.UpdatedAt)
            .HasDefaultValueSql("GETUTCDATE()")
            .ValueGeneratedOnAddOrUpdate();

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Car> Cars { get; set; }
    public DbSet<Make> Makes { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Location> Locations { get; set; }

}