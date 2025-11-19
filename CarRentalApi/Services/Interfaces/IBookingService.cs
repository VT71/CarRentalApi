using CarRentalApi.Models;

namespace CarRentalApi.Services.Interfaces;

public interface IBookingService
{
    Task<PaginatedList<Booking>> GetAllAsync(PaginatedQuery query);
    Task<PaginatedList<Booking>> GetByCustomerIdAsync(string customerId, PaginatedQuery query);
    Task<Booking?> Create(Booking newBooking);
    Task<Booking?> GetById(long id);
    Task<bool> Update(long id, Booking booking);
    Task Delete(Booking booking);
    string[] GetAllCountries();
}