using CarRentalApi.Models;
using CarRentalApi.Models.Dtos.Make;

namespace CarRentalApi.Extensions;

public static class MakeMappingExtensions
{
    public static MakeDto ToDto(this Make make)
    {
        return new MakeDto
        {
            Id = make.Id,
            Name = make.Name,
            Cars = make.Cars
        };
    }
}
