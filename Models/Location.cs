namespace CarRentalApi.Models;

public class Location
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public bool Available { get; set; }
    public ICollection<Booking> Bookings { get; } = new List<Booking>();

}