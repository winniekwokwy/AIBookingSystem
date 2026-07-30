using AIBookingSystem.Models;
namespace AIBookingSystem.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user, IList<string> roles, out string jwtId, Client client);
        RefreshToken GenerateRefreshToken(string ipAddress, string jwtId, Client client, int userId);
        void AddRefreshTokens(RefreshToken refreshToken);

        RefreshToken? GetExistingToken(string refreshToken, int ClientId);

        bool RevokeRefreshToken(string refreshToken);

        void RevokeToken(RefreshToken token);

        RefreshToken? GetExistingToken(string refreshToken);
    }
}