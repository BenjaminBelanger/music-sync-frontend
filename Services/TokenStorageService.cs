using CredentialManagement;
using System;

namespace music_sync_frontend.Services
{
    public class TokenStorageService
    {
        private const string AccessTokenKey = "Spotify_Access_Token";
        private const string RefreshTokenKey = "Spotify_Refresh_Token";

        public void SaveTokens(string? accessToken, string? refreshToken)
        {   
            if (accessToken != null)
            {
                SaveToken(AccessTokenKey, accessToken);
            }
            if (refreshToken != null) {
                SaveToken(RefreshTokenKey, refreshToken);
            }
        }

        public (string? AccessToken, string? RefreshToken) GetTokens()
        {
            return (
                GetToken(AccessTokenKey),
                GetToken(RefreshTokenKey)
            );
        }

        public void DeleteTokens()
        {
            DeleteToken(AccessTokenKey);
            DeleteToken(RefreshTokenKey);
        }

        private void SaveToken(string key, string token)
        {
            using var cred = new Credential
            {
                Target = key,
                Username = "Token",
                Password = token,
                PersistanceType = PersistanceType.LocalComputer
            };
            cred.Save();
        }

        private string? GetToken(string key)
        {
            using var cred = new Credential { Target = key };
            return cred.Load() ? cred.Password : null;
        }

        private void DeleteToken(string key)
        {
            using var cred = new Credential { Target = key };
            cred.Delete();
        }
    }
}
