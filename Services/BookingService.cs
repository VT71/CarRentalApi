using CarRentalApi.Data;
using CarRentalApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CarRentalApi.Services;

public class BookingService
{
    private readonly CarRentalContext _context;

    private readonly string[] countries = ["Afghanistan", "Albania", "Algeria", "Andorra", "Angola", "Antigua and Barbuda",
    "Argentina", "Armenia", "Australia", "Austria", "Azerbaijan", "Bahamas",
    "Bahrain", "Bangladesh", "Barbados", "Belarus", "Belgium", "Belize", "Benin",
    "Bhutan", "Bolivia", "Bosnia and Herzegovina", "Botswana", "Brazil",
    "Brunei Darussalam", "Bulgaria", "Burkina Faso", "Burundi", "Cabo Verde",
    "Cambodia", "Cameroon", "Canada", "Central African Republic", "Chad", "Chile",
    "China", "Colombia", "Comoros", "Congo", "Costa Rica", "Croatia", "Cuba",
    "Cyprus", "Czechia", "Denmark", "Djibouti", "Dominica", "Dominican Republic",
    "Ecuador", "Egypt", "El Salvador", "Equatorial Guinea", "Eritrea", "Estonia",
    "Eswatini", "Ethiopia", "Fiji", "Finland", "France", "Gabon", "Gambia", "Georgia",
    "Germany", "Ghana", "Greece", "Grenada", "Guatemala", "Guinea",
    "Guinea-Bissau", "Guyana", "Haiti", "Honduras", "Hungary", "Iceland", "India",
    "Indonesia", "Iran", "Iraq", "Ireland", "Israel", "Italy", "Jamaica", "Japan",
    "Jordan", "Kazakhstan", "Kenya", "Kiribati", "Kuwait", "Kyrgyzstan", "Lao PDR",
    "Latvia", "Lebanon", "Lesotho", "Liberia", "Libya", "Liechtenstein",
    "Lithuania", "Luxembourg", "Madagascar", "Malawi", "Malaysia", "Maldives",
    "Mali", "Malta", "Marshall Islands", "Mauritania", "Mauritius", "Mexico",
    "Micronesia", "Moldova", "Monaco", "Mongolia", "Montenegro", "Morocco",
    "Mozambique", "Myanmar", "Namibia", "Nauru", "Nepal", "Netherlands",
    "New Zealand", "Nicaragua", "Niger", "Nigeria", "North Macedonia", "Norway",
    "Oman", "Pakistan", "Palau", "Panama", "Papua New Guinea", "Paraguay", "Peru",
    "Philippines", "Poland", "Portugal", "Qatar", "Romania", "Russia", "Rwanda",
    "Saint Kitts and Nevis", "Saint Lucia", "Saint Vincent and the Grenadines",
    "Samoa", "San Marino", "Sao Tome and Principe", "Saudi Arabia", "Senegal",
    "Serbia", "Seychelles", "Sierra Leone", "Singapore", "Slovakia", "Slovenia",
    "Solomon Islands", "Somalia", "South Africa", "South Sudan", "Spain",
    "Sri Lanka", "Sudan", "Suriname", "Sweden", "Switzerland", "Syria", "Taiwan",
    "Tajikistan", "Tanzania", "Thailand", "Timor-Leste", "Togo", "Tonga",
    "Trinidad and Tobago", "Tunisia", "Turkey", "Turkmenistan", "Tuvalu",
    "Uganda", "Ukraine", "United Arab Emirates", "United Kingdom",
    "United States of America", "Uruguay", "Uzbekistan", "Vanuatu", "Venezuela",
    "Vietnam", "Yemen", "Zambia", "Zimbabwe"];

    public BookingService(CarRentalContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Booking>> GetAll()
    {
        return await _context.Bookings.AsNoTracking().ToListAsync(); ;
    }

    public async Task<Booking?> Create(Booking newBooking)
    {
        Booking? validatedBooking = ValidateBooking(newBooking);

        if (validatedBooking != null)
        {
            await _context.AddAsync(validatedBooking);
            await _context.SaveChangesAsync();
            return validatedBooking;
        }

        return null;
    }

    public async Task<Booking?> GetById(long id)
    {
        return await _context.Bookings.FindAsync(id);
    }

    public async Task<bool> Update(long id, Booking booking)
    {
        _context.Entry(booking).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!BookingExists(id))
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

    public async Task Delete(Booking booking)
    {
        _context.Bookings.Remove(booking);
        await _context.SaveChangesAsync();
    }

    private Booking? ValidateBooking(Booking booking)
    {
        var car = _context.Cars.Find(booking.CarId);
        if (car != null && car.Available == true && !BookingOverlap(booking))
        {
            booking.Car = car;
        }
        else
        {
            return null;
        }


        var nowTime = DateTimeOffset.UtcNow;
        if (DateTimeOffset.Compare(booking.PickUpDateTime, nowTime) <= 0)
        {
            return null;
        }

        if (DateTimeOffset.Compare(booking.DropOffDateTime, booking.PickUpDateTime) <= 0)
        {
            return null;
        }

        var pickUpLocation = _context.Locations.Find(booking.PickUpLocationId);
        if (pickUpLocation != null && pickUpLocation.PickUpAvailable == true)
        {
            booking.PickUpLocation = pickUpLocation;

        }
        else
        {
            return null;
        }

        var dropOffLocation = _context.Locations.Find(booking.DropOffLocationId);
        if (dropOffLocation != null && dropOffLocation.DropOffAvailable == true)
        {
            booking.DropOffLocation = dropOffLocation;
        }
        else
        {
            return null;
        }

        booking.Status = Status.Pending;

        return booking;
    }

    public string[] GetAllCountries()
    {
        return countries;
    }

    private bool ValidStatus(object? value)
    {
        if (value != null && Enum.IsDefined(typeof(Status), value))
        {
            return true;
        }
        return false;
    }

    private bool BookingOverlap(Booking newBooking)
    {
        return _context.Bookings.Any(b => b.CarId == newBooking.CarId && DateTimeOffset.Compare(newBooking.PickUpDateTime, b.DropOffDateTime) < 0 && DateTimeOffset.Compare(newBooking.DropOffDateTime, b.PickUpDateTime) > 0);
    }

    private bool BookingExists(long id)
    {
        return _context.Bookings.Any(e => e.Id == id);
    }

}