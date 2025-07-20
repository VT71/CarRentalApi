namespace CarRentalApi.Models;

public class Location
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public bool PickUpAvailable { get; set; }
    public bool DropOffAvailable { get; set; }
    public ICollection<Booking> PickUpBookings { get; } = new List<Booking>();
    public ICollection<Booking> DropOffBookings { get; } = new List<Booking>();

}