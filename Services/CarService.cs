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

    public async Task<Car?> GetById(long id)
    {
        var car = await _context.Cars.AsNoTracking().Include(car => car.Make).SingleOrDefaultAsync(c => c.Id == id);

        return car;
    }

    public async Task<bool> Update(long id, Car car)
    {
        _context.Entry(car).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CarExists(id))
            {
                return false;
            }
            else
            {
                throw;
            }
        }

        return true;
    }

    private bool CarExists(long id)
    {
        return _context.Cars.Any(e => e.Id == id);
    }
}