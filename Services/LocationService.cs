using CarRentalApi.Data;
using CarRentalApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CarRentalApi.Services;

public class LocationService
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
}