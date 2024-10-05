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
        return FilterAvailableCars(await _context.Cars.AsNoTracking().Include(car => car.Make).ToListAsync());
    }

    public async Task<Car?> GetById(long id)
    {
        var car = await _context.Cars.AsNoTracking().Include(car => car.Make).SingleOrDefaultAsync(c => c.Id == id && c.Available == true);

        return car;
    }

    public async Task<IEnumerable<Car>> GetAvailableCars(long pickUpLocationId, long dropOffLocationId, string pickUpDateTimeIso, string dropOffDateTimeIso)
    {
        DateTimeOffset pickUpDateTime;
        DateTimeOffset dropOffDateTime;
        var nowTime = DateTimeOffset.UtcNow;

        bool validPickUpDateTime = DateTimeOffset.TryParse(pickUpDateTimeIso, out pickUpDateTime) && DateTimeOffset.Compare(pickUpDateTime, nowTime) > 0;
        bool validDropOffDateTime = DateTimeOffset.TryParse(dropOffDateTimeIso, out dropOffDateTime) && DateTimeOffset.Compare(dropOffDateTime, pickUpDateTime) > 0;

        var pickUpLocation = await _context.Locations.AsNoTracking().SingleOrDefaultAsync(l => l.Id == pickUpLocationId);
        var dropOffLocation = await _context.Locations.AsNoTracking().SingleOrDefaultAsync(l => l.Id == dropOffLocationId);

        if (pickUpLocation != null && pickUpLocation.PickUpAvailable
        && dropOffLocation != null && dropOffLocation.DropOffAvailable
        && validPickUpDateTime && validDropOffDateTime)
        {
            List<long> overlappingBookingCarIds = await _context.Bookings.Where(b => DateTimeOffset.Compare(pickUpDateTime, b.DropOffDateTime) < 0 && DateTimeOffset.Compare(dropOffDateTime, b.PickUpDateTime) > 0).Select(b => b.Id).ToListAsync();

            if (overlappingBookingCarIds.Count > 0)
            {
                return FilterAvailableCars(await _context.Cars.Where(c => !overlappingBookingCarIds.Contains(c.Id)).Include(c => c.Make).ToListAsync());
            }
            else
            {
                return FilterAvailableCars(await _context.Cars.AsNoTracking().ToListAsync());
            }
        }

        return [];
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

    public async Task<Car?> Create(Car car)
    {
        var make = await _context.Makes.SingleOrDefaultAsync(m => m.Id == car.MakeId);

        if (make == null)
        {
            return null;
        }

        make.Cars.Add(car);

        await _context.SaveChangesAsync();

        return car;
    }

    public async Task Delete(Car car)
    {
        _context.Cars.Remove(car);
        await _context.SaveChangesAsync();
    }

    private bool CarExists(long id)
    {
        return _context.Cars.Any(e => e.Id == id);
    }

    private IEnumerable<Car> FilterAvailableCars(List<Car> cars)
    {
        return cars.Where(c => c.Available).ToList();
    }
}