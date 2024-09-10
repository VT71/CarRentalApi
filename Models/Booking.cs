using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRentalApi.Models;

public class Booking
{
    public long Id { get; set; }
    public required string UserId {get; set; }
    public DateTimeOffset PickUpDateTime { get; set; }
    public DateTimeOffset DropOffDateTime { get; set; }
    public long CarId { get; set; }
    public required Car Car { get; set; }
}