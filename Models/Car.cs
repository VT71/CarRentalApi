using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRentalApi.Models;

public class Car
{
    public long Id { get; set; }
    [Required]
    public string? Make { get; set; }
    [Required]
    public string? Model { get; set; }
    [Required]
    public string? Description { get; set; }
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Deposit { get; set; }
    public int Seats { get; set; }
    public int Doors { get; set; }
    [Required]
    public string? TransmissionType { get; set; }
    public int PowerHp { get; set; }
    public int RangeKm { get; set; }
    public AvailabilityStatus AvailabilityStatus { get; set; }
}

public enum AvailabilityStatus
{
    Available, Maintenance
}