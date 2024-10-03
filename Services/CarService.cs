using CarRentalApi.Data;
using CarRentalApi.Models;
using Microsoft.EntityFrameworkCore;

public class CarService
{
    private readonly CarRentalContext _context;

    public CarService(CarRentalContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Car>> GetAll()
    {
        return await _context.Cars.AsNoTracking().Include(car => car.Make).ToListAsync();
    }
}