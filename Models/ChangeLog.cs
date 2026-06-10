using System.ComponentModel.DataAnnotations.Schema;

public class ChangeLog
{
    public int Id;
    public required string EntityType { get; set;}
    public required int UserId;
    public required User User {get; set;}
    [ForeignKey("Id")]
    public required string Action {get; set;}
    public required DateTime ChangedTime { get; set;}
}