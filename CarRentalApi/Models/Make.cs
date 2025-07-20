namespace CarRentalApi.Models;

public class Make
{
    public long Id { get; set; }
    public required string Name {get; set; }
    public ICollection<Car> Cars {get; } = new List<Car>();
}