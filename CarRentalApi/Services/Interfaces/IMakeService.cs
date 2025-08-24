using CarRentalApi.Models;
using CarRentalApi.Models.Dtos.Make;

namespace CarRentalApi.Services.Interfaces;

public interface IMakeService
{
    Task<IEnumerable<MakeDto>> GetAll();
    Task<Make> Create(Make newMake);
    Task<Make?> GetById(long id);
    Task<bool> Update(long id, Make make);
    Task<bool> Delete(long id);
}