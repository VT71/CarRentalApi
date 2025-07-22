namespace CarRentalApi.Models;

public class Booking
{
    public long Id { get; set; }
    public required string UserId { get; set; }
    public long CarId { get; set; }
    public required Car Car { get; set; }
    public DateTimeOffset PickUpDateTime { get; set; }
    public DateTimeOffset DropOffDateTime { get; set; }
    public long PickUpLocationId { get; set; }
    public required Location PickUpLocation { get; set; }
    public long DropOffLocationId { get; set; }
    public required Location DropOffLocation { get; set; }
    public required BookingStatus Status {get; set;}
}

public enum BookingStatus
{
    Pending,
    Confirmed,
    Cancelled,
}