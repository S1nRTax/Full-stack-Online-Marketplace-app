using Server.Models;

namespace Server.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken(int size =64);
        Task<string> RetrieveUseridByRefreshToken(string refreshToken);
        Task SaveAccessTokenAsync(User user, string token);
        void SetAccessToken(AccessToken refreshToken, User user);
    }
}
