using music_sync_frontend.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace music_sync_frontend.Services
{
    public class SpotifyService
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenStorageService _tokenService;

        public SpotifyService(HttpClient httpClient, ITokenStorageService tokenStorage)
        {
            _httpClient = httpClient;
            _tokenService = tokenStorage;

            _httpClient.BaseAddress = new Uri("https://api.spotify.com/v1/");
        }

        public async Task<string> GetCurrentlyPlayingTrackAsync()
        {
            return await SendRequestAsync<string>(HttpMethod.Get, "me/player/currently-playing");
        }
        

        public async Task<User> GetCurrentUserProfileAsync()
        {
            return await SendRequestAsync<User>(HttpMethod.Get, "me");
        }

        private async Task<T> SendRequestAsync<T>(HttpMethod method, string endpoint, object content = null)
        {
            var (token, _) = _tokenService.GetTokens();

            var request = new HttpRequestMessage(method, endpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            if (content != null)
            {
                request.Content = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");
            }

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            if (typeof(T) == typeof(object))
            {
                return default;
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(responseContent);
        }
    }
}
