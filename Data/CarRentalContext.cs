using Microsoft.EntityFrameworkCore;
using CarRentalApi.Models;

namespace CarRentalApi.Data;

public class CarRentalContext : DbContext
{
    public CarRentalContext(DbContextOptions<CarRentalContext> options) : base(options)
    {
    }

    public DbSet<Car> Cars { get; set; }
    public DbSet<Make> Make { get; set; }

}