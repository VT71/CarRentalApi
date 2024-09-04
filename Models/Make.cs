using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CarRentalApi.Models;

public class Make
{
    public long Id { get; set; }
    public required string Name {get; set; }
    [JsonIgnore]
    public ICollection<Car> Cars {get; } = new List<Car>();
}