using AIBookingSystem.Data;
using AIBookingSystem.DTO;
using AIBookingSystem.Enums;
using AIBookingSystem.Models;

namespace AIBookingSystem.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        private readonly RoomBookingDbContext _dBContext;
        public TokenRepository(RoomBookingDbContext dBContext)
        {
            _dBContext = dBContext;
        }

        public void AddRefreshTokens(RefreshToken refreshToken)
        {
            _dBContext.RefreshTokens.Add(refreshToken);
            _dBContext.SaveChanges();
        }

                // Refreshes an expired access token using a valid refresh token and client ID
        public RefreshToken? GetExistingToken(string refreshToken, int clientId)
        {
            // Look up the refresh token in database, including related user and roles for new token generation
            var existingToken = _dBContext.RefreshTokens
                .FirstOrDefault(rt => rt.Token == refreshToken && rt.ClientId == clientId);
            // Validate refresh token existence, revocation status, and expiration
            if (existingToken == null || existingToken.IsRevoked || existingToken.Expires <= DateTime.UtcNow)
                return null; // Invalid refresh token
            // Revoke old refresh token immediately to prevent reuse
            existingToken.IsRevoked = true;
            existingToken.RevokedAt = DateTime.UtcNow;
            _dBContext.SaveChanges();

            return existingToken;
        }

        public bool RevokeRefreshToken(string refreshToken)
        {
            // Look up the refresh token in the database
            var existingToken = _dBContext.RefreshTokens.FirstOrDefault(rt => rt.Token == refreshToken);
            // Return false if token not found or already revoked
            if (existingToken == null || existingToken.IsRevoked)
                return false;
            // Mark token as revoked and record revocation time
            existingToken.IsRevoked = true;
            existingToken.RevokedAt = DateTime.UtcNow;
            // Persist changes to database
            _dBContext.SaveChanges();
            return true; // Indicate successful revocation
        }
    }
}