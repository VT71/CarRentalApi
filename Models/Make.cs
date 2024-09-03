using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRentalApi.Models;

public class Make
{
    public long Id { get; set; }
    public string? Name {get; set; }
    public ICollection<Car> Cars {get; } = new List<Car>();
}