using CredentialManagement;
using System;

namespace music_sync_frontend.Services
{
    public class TokenStorageService
    {
        private const string AccessTokenSuffix = "_access";
        private const string RefreshTokenSuffix = "_refresh";

        public void SaveTokens(string key, string accessToken, string refreshToken)
        {
            using var accessCred = new Credential
            {
                Target = key + AccessTokenSuffix,
                Username = "SpotifyAccessToken",
                Password = accessToken,
                PersistanceType = PersistanceType.LocalComputer
            };
            accessCred.Save();

            using var refreshCred = new Credential
            {
                Target = key + RefreshTokenSuffix,
                Username = "SpotifyRefreshToken",
                Password = refreshToken,
                PersistanceType = PersistanceType.LocalComputer
            };
            refreshCred.Save();
        }

        public (string? AccessToken, string? RefreshToken) GetTokens(string key)
        {
            string? accessToken = null;
            string? refreshToken = null;

            using (var accessCred = new Credential { Target = key + AccessTokenSuffix })
            {
                if (accessCred.Load())
                {
                    accessToken = accessCred.Password;
                }
            }

            using (var refreshCred = new Credential { Target = key + RefreshTokenSuffix })
            {
                if (refreshCred.Load())
                {
                    refreshToken = refreshCred.Password;
                }
            }

            return (accessToken, refreshToken);
        }

        public void DeleteTokens(string key)
        {
            using (var accessCred = new Credential { Target = key + AccessTokenSuffix })
            {
                accessCred.Delete();
            }

            using (var refreshCred = new Credential { Target = key + RefreshTokenSuffix })
            {
                refreshCred.Delete();
            }
        }

        public void SaveAccessToken(string key, string token)
        {
            SaveToken(key + AccessTokenSuffix, token, "SpotifyAccessToken");
        }

        public void SaveRefreshToken(string key, string token)
        {
            SaveToken(key + RefreshTokenSuffix, token, "SpotifyRefreshToken");
        }

        public string? GetAccessToken(string key)
        {
            return GetToken(key + AccessTokenSuffix);
        }

        public string? GetRefreshToken(string key)
        {
            return GetToken(key + RefreshTokenSuffix);
        }

        private void SaveToken(string fullKey, string token, string username)
        {
            using var cred = new Credential
            {
                Target = fullKey,
                Username = username,
                Password = token,
                PersistanceType = PersistanceType.LocalComputer
            };
            cred.Save();
        }

        private string? GetToken(string fullKey)
        {
            using var cred = new Credential { Target = fullKey };
            return cred.Load() ? cred.Password : null;
        }
    }
}
