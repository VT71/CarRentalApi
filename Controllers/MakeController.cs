using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarRentalApi.Data;
using CarRentalApi.Models;

namespace CarRentalApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MakeController : ControllerBase
    {
        private readonly CarRentalContext _context;

        public MakeController(CarRentalContext context)
        {
            _context = context;
        }

        // GET: api/Make
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Make>>> GetMake()
        {
            return await _context.Make.AsNoTracking().ToListAsync();
        }

        // GET: api/Make/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Make>> GetMake(long id)
        {
            var make = await _context.Make.AsNoTracking().Include(m => m.Cars).SingleOrDefaultAsync(m => m.Id == id);

            if (make == null)
            {
                return NotFound();
            }

            return make;
        }

        // PUT: api/Make/5
        [HttpPut]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateName(long id, string name)
        {
            var make = await _context.Make.SingleOrDefaultAsync(m => m.Id == id);

            if (make == null)
            {
                return BadRequest();
            }

            make.Name = name;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict();
            }

            return NoContent();
        }

        // PUT: api/Make/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // [HttpPut("{id}")]
        // public async Task<IActionResult> PutMake(long id, Make make)
        // {
        //     if (id != make.Id)
        //     {
        //         return BadRequest();
        //     }

        //     _context.Entry(make).State = EntityState.Modified;

        //     try
        //     {
        //         await _context.SaveChangesAsync();
        //     }
        //     catch (DbUpdateConcurrencyException)
        //     {
        //         if (!MakeExists(id))
        //         {
        //             return NotFound();
        //         }
        //         else
        //         {
        //             throw;
        //         }
        //     }

        //     return NoContent();
        // }

        // POST: api/Make
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Make>> PostMake(Make make)
        {
            _context.Make.Add(make);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMake", new { id = make.Id }, make);
        }

        // DELETE: api/Make/5
        // [HttpDelete("{id}")]
        // public async Task<IActionResult> DeleteMake(long id)
        // {
        //     var make = await _context.Make.FindAsync(id);
        //     if (make == null)
        //     {
        //         return NotFound();
        //     }

        //     _context.Make.Remove(make);
        //     await _context.SaveChangesAsync();

        //     return NoContent();
        // }

        private bool MakeExists(long id)
        {
            return _context.Make.Any(e => e.Id == id);
        }
    }
}
