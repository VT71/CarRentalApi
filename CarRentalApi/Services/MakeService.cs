using CarRentalApi.Data;
using CarRentalApi.Models;
using CarRentalApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarRentalApi.Services;
    
public class MakeService: IMakeService
    {
    private readonly CarRentalContext _context;

    public MakeService(CarRentalContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Make>> GetAll()
    {
        return await _context.Makes.AsNoTracking()
                .Include(m => m.Cars)
                .ToListAsync();
    }

    public async Task<Make?> GetById(long id)
    {
        return await _context.Makes.FindAsync(id);
    }

    public async Task<Make> Create(Make make)
    {
        _context.Makes.Add(make);
        await _context.SaveChangesAsync();
        return make;
    }

    public async Task<bool> Update(long id, Make make)
    {
        if (id != make.Id)
        {
            return false;
        }

        _context.Entry(make).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> Delete(long id)
    {
        var make = await _context.Makes.FindAsync(id);

        if (make == null)
        {
            return false;
        }

        _context.Makes.Remove(make);
        await _context.SaveChangesAsync();
        return true;
    }
}