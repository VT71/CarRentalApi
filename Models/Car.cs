namespace CarRentalApi.Models;

public class Car
{
    public long Id { get; set; }
    public string? Make { get; set; }
    public string? Model { get; set; }
    public string? Description { get; set; }
    public decimal Deposit { get; set; }
    public int Seats { get; set; }
    public int Doors { get; set; }
    public string? TransmissionType { get; set; }
    public int PowerHp { get; set; }
    public int RangeKm { get; set; }
    public AvailabilityStatus AvailabilityStatus { get; set; }
}

public enum AvailabilityStatus
{
    Available, Rented, Maintenance
}