using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace music_sync_frontend.Services
{
    public interface ISpotifyAuthService
    {
        Task<string> AuthenticateAsync();
    }

}
