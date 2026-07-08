using NodaTime;


namespace AIBookingSystem.Services
{
    public class TimeService
    {
        private static readonly Random random = new Random();
        public static DateTimeOffset GenerateRandomDate(DateTimeOffset from, DateTimeOffset to)
        {
            int totalDays = to.Subtract(from).Days;
            return from.AddDays(random.Next(totalDays+1));
        }

        public static DateTimeOffset GenerateRandomDateTime(DateTimeOffset from, DateTimeOffset to)
        {
            var date = GenerateRandomDate(from, to);
            var hour = random.Next(24);
            var min = random.Next(60);

            return new DateTimeOffset(date.Year, date.Month, date.Day, hour, min, 0, TimeSpan.Zero);
        }
    }
}