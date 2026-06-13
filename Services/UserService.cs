using Microsoft.AspNetCore.Mvc;

public class UserService
{
    public static bool IsRoleValid(string role)
    {
        if ((role.ToLower() != "user") && role.ToLower() != "admin")
        {
            return false;
        }
        return true;
    }

    public static bool UsernameExsited(
        string username,
        [FromServices] RoomBookingDbContext dBContext)
    {
        if (dBContext.Users.FirstOrDefault(u => u.UserName == username) != null)
        {
            return true;
        }
        return false;
    }

    public static bool IsUserValid(
        int id,
        [FromServices] RoomBookingDbContext dBContext)
    {
        if (dBContext.Users.FirstOrDefault(u => u.Id == id) != null)
        {
            return true;
        }
        return false;
    }
}