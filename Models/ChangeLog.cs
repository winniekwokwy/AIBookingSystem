using System.ComponentModel.DataAnnotations.Schema;
using NodaTime;

public class ChangeLog
{
    public int Id {get;set;}
    public required string EntityType { get; set;}
    public int UserId { get; set;}
    public User? User {get; set;}
    [ForeignKey("Id")]
    public required string Action {get; set;}
    public Instant ChangedTime { get; set;} = Instant.FromDateTimeUtc(DateTime.UtcNow);
}