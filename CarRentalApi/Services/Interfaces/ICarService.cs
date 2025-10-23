using CarRentalApi.Models;

namespace CarRentalApi.Services.Interfaces
{
    public interface ICarService
    {
        Task<IEnumerable<Car>> GetAll();
        Task<Car?> GetById(long id);
        Task<IEnumerable<Car>> GetAvailableCars(long pickUpLocationId, long dropOffLocationId, string pickUpDateTimeIso, string dropOffDateTimeIso);
        Task<Car?> Create(Car car);
        Task<bool> Update(long id, Car car);
        Task Delete(Car car);
    }
}