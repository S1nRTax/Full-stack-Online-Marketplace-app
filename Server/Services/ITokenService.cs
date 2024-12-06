using Server.Models;

namespace Server.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken(int size =64);
        Task<string> RetrieveUseridByRefreshToken(string refreshToken);
        Task SaveRefreshTokenAsync(User user, RefreshToken refreshToken);
        void SetRefreshToken(RefreshToken refreshToken, User user);
    }
}
