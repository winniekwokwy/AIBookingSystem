using AIBookingSystem.Data;
using AIBookingSystem.DTO;
using AIBookingSystem.Enums;
using AIBookingSystem.Models;
using Microsoft.EntityFrameworkCore;

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
            return _dBContext.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefault(rt => rt.Token == refreshToken && rt.ClientId == clientId);

        }

        public bool RevokeRefreshToken(string refreshToken)
        {
            // Look up the refresh token in the database
            var existingToken = GetExistingToken(refreshToken);
            // Return false if token not found or already revoked
            if (existingToken == null || existingToken.IsRevoked)
                return false;
            // Mark token as revoked and record revocation time
            RevokeToken(existingToken);
            return true; // Indicate successful revocation
        }

        public void RevokeToken(RefreshToken token)
        {
            token.IsRevoked = true;
            token.RevokedAt = DateTime.UtcNow;
            _dBContext.SaveChanges();
        }

        public RefreshToken? GetExistingToken(string refreshToken)
        {
            return _dBContext.RefreshTokens.FirstOrDefault(rt => rt.Token == refreshToken);
        }

        public async Task<RefreshToken?> GetAccessTokenByJtiAsync(string jti)
        {
            return await _dBContext.RefreshTokens
                .AsNoTracking()
                .FirstOrDefaultAsync(rt => rt.JwtId == jti);
        }
    }
}