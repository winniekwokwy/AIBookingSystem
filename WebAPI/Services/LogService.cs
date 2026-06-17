using AIBookingSystem.DTO;

namespace AIBookingSystem.Services
{
    public class LogService : ILogService
    {
        
        private readonly RoomBookingDbContext _dBContext;
        private readonly ILogger<LogService> _logger;

        public LogService(RoomBookingDbContext dBContext, ILogger<LogService> logger)
        {
            _dBContext = dBContext;
            _logger = logger;
        }
        public bool AddUserChangeLog(UserCreateDTO user)
        {
            var newLog = new ChangeLog
                            {
                                EntityType = "User",
                                UserId = user.UserId,
                                ChangedBy = user.CreatedBy,
                                Action = "Add"
                            };
            try {
                _dBContext.ChangeLogs.Add(newLog);
                _dBContext.SaveChanges();
                return true;
            } 
            catch (Exception e){
                _logger.LogError($"LogService.AddUserChangeLog(): Change Log update is failed. {e}");
                return false;
            }
        }
    }
}