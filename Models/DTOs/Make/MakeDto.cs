namespace CarRentalApi.Models.Dtos.Make;

public class MakeDto
{
    public required long Id { get; set; }
    public required string Name {get; set; }
    public required ICollection<Car> Cars {get; set; }
}