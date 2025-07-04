using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace music_sync_frontend.Services
{
    public interface ITokenStorageService
    {
        void SaveTokens(string? accessToken, string? refreshToken);
        (string? AccessToken, string? RefreshToken) GetTokens();
        void DeleteTokens();
    }
}
