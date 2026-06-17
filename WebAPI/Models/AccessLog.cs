using System.ComponentModel.DataAnnotations.Schema;
using NodaTime;

public class AccessLog
{
    public int Id {get; set;}

    public required string AccessedBy {get; set;}

    [ForeignKey("Id")]
    public required int UserId {get; set;}
    public required User User {get; set;}
    public required string Status {get; set;}
    public required string Reason {get; set;}
    public Instant AccessedTime { get; set;} = Instant.FromDateTimeUtc(DateTime.UtcNow);
}