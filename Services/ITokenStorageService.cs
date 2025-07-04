using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace music_sync_frontend.Services
{
    public interface ITokenStorageService
    {
        void SaveToken(string key, string token);
        string? GetToken(string key);
        void DeleteToken(string key);
    }
}
