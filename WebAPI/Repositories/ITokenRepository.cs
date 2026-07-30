
using System.ComponentModel;
using AIBookingSystem.Models;

namespace AIBookingSystem.Repositories
{
    public interface ITokenRepository
    {
        void AddRefreshTokens(RefreshToken refreshToken);

        RefreshToken? GetExistingToken(string refreshToken, int ClientId);

        bool RevokeRefreshToken(string refreshToken);

        void RevokeToken(RefreshToken token);

        RefreshToken? GetExistingToken(string refreshToken);
    }
}