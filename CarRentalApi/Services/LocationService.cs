using CareRentalApi.Services.Interfaces;
using CarRentalApi.Data;
using CarRentalApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CarRentalApi.Services;

public class LocationService: ILocationService
{

    private readonly CarRentalContext _context;


    public LocationService(CarRentalContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Location>> GetAll()
    {
        return await _context.Locations.AsNoTracking().ToListAsync();
    }

    public async Task<IEnumerable<Location>> GetAllDropOff()
    {
        return await _context.Locations.AsNoTracking().Where(l => l.DropOffAvailable == true).ToListAsync();
    }

    public async Task<IEnumerable<Location>> GetAllPickUp()
    {
        return await _context.Locations.AsNoTracking().Where(l => l.PickUpAvailable == true).ToListAsync();
    }

    public async Task<Location?> GetById(long id)
    {
        return await _context.Locations.FindAsync(id);
    }

    public async Task<bool> Update(long id, Location location)
    {
        _context.Entry(location).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!LocationExists(id))
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

    public async Task<Location> Create(Location location)
    {
        await _context.Locations.AddAsync(location);
        await _context.SaveChangesAsync();

        return location;
    }

    public async Task Delete(Location location)
    {
        _context.Locations.Remove(location);
        await _context.SaveChangesAsync();
    }

    private bool LocationExists(long id)
    {
        return _context.Locations.Any(e => e.Id == id);
    }
}