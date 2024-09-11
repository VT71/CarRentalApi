using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRentalApi.Models;

public class Car
{
    public long Id { get; set; }
    public long MakeId { get; set; }
    public required Make Make { get; set; }
    public required string Model { get; set; }
    public required string Description { get; set; }
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Deposit { get; set; }
    public int Seats { get; set; }
    public int Doors { get; set; }
    public required TransmissionType TransmissionType { get; set; }
    public int PowerHp { get; set; }
    public int RangeKm { get; set; }
    public bool Available { get; set; }
    public ICollection<Booking> Bookings { get; } = new List<Booking>();
}

public enum TransmissionType
{
    Auto = 0, Manual = 1
}