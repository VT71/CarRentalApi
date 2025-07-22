using CarRentalApi.Models;

namespace CarRentalApi.Services.Interfaces;

public interface IBookingService
{
    Task<IEnumerable<Booking>> GetAll();
    Task<Booking?> Create(Booking newBooking);
    Task<Booking?> GetById(long id);
    Task<bool> Update(long id, Booking booking);
    Task Delete(Booking booking);
    string[] GetAllCountries();
}