using CarRentalApi.Models;

namespace CareRentalApi.Services.Interfaces
{
    public interface ILocationService
    {
        Task<IEnumerable<Location>> GetAll();
        Task<Location?> GetById(long id);
        Task<IEnumerable<Location>> GetAllDropOff();
        Task<IEnumerable<Location>> GetAllPickUp();
        Task<Location> Create(Location location);
        Task<bool> Update(long id, Location location);
        Task Delete(Location location);
    }
}