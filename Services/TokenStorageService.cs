using CredentialManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace music_sync_frontend.Services
{
    public class TokenStorageService
    {
        public void SaveToken(string key, string token)
        {
            using var cred = new Credential
            {
                Target = key,
                Username = "SpotifyAuthToken",
                Password = token,
                PersistanceType = PersistanceType.LocalComputer
            };
            cred.Save();
        }

        public string? GetToken(string key)
        {
            using var cred = new Credential { Target = key };
            return cred.Load() ? cred.Password : null;
        }

        public void DeleteToken(string key)
        {
            using var cred = new Credential { Target = key };
            cred.Delete();
        }
    }
}
