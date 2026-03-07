namespace CarRentalApi.Models;

public class BookingPatchDto
{
    public required long Id { get; set; }
    public required BookingStatus Status {get; set;}
}