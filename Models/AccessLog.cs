using System.ComponentModel.DataAnnotations.Schema;

public class AccessLog
{
    public int Id;
    public required int UserId;
    public required User User {get; set;}
    [ForeignKey("Id")]
    public required string Status {get; set;}
    public required string Reason {get; set;}
    public required DateTime AccessedTime { get; set;}
}