using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace music_sync_frontend.Services
{
    public class SpotifyAuthService : ISpotifyAuthService
    {
        private readonly string clientID = ConfigurationManager.AppSettings["CliendId"];
        private readonly string redirectUri = $"http://{IPAddress.Loopback}:{ConfigurationManager.AppSettings["RedirectUriPort"]}/";
        private const string AuthorizationEndpoint = "https://accounts.spotify.com/authorize";
        private const string TokenEndpoint = "https://accounts.spotify.com/api/token";

        public async Task<string> AuthenticateAsync()
        {
            string state = GenerateRandomBase64Url(32);
            string codeVerifier = GenerateRandomBase64Url(32);
            string codeChallenge = Base64UrlEncodeNoPadding(SHA256Hash(codeVerifier));
            string authorizationRequest = $"{AuthorizationEndpoint}?response_type=code&client_id={clientID}&redirect_uri={redirectUri}&code_challenge={codeChallenge}&code_challenge_method=S256";

            using (var http = new HttpListener())
            {
                http.Prefixes.Add(redirectUri);
                http.Start();

                Process.Start(new ProcessStartInfo { FileName = authorizationRequest, UseShellExecute = true });

                var context = await http.GetContextAsync();
                string code = context.Request.QueryString.Get("code");

                byte[] buffer = Encoding.UTF8.GetBytes("<html><body>Login complete. Return to the app.</body></html>");
                context.Response.ContentLength64 = buffer.Length;
                await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                context.Response.OutputStream.Close();
                http.Stop();

                if (string.IsNullOrEmpty(code)) return null;

                return await ExchangeCodeForTokenAsync(code, codeVerifier);
            }
        }

        private async Task<string> ExchangeCodeForTokenAsync(string code, string codeVerifier)
        {
            var body = $"code={code}&redirect_uri={Uri.EscapeDataString(redirectUri)}&client_id={clientID}&code_verifier={codeVerifier}&grant_type=authorization_code";
            var request = WebRequest.CreateHttp(TokenEndpoint);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            byte[] byteBody = Encoding.ASCII.GetBytes(body);
            using (var stream = await request.GetRequestStreamAsync())
            {
                await stream.WriteAsync(byteBody, 0, byteBody.Length);
            }

            try
            {
                using var response = await request.GetResponseAsync();
                using var reader = new StreamReader(response.GetResponseStream());
                var json = await reader.ReadToEndAsync();
                var token = JsonConvert.DeserializeObject<Dictionary<string, string>>(json)["access_token"];
                return token;
            }
            catch (WebException ex)
            {
                using var errorResponse = ex.Response;
                if (errorResponse != null)
                {
                    using var reader = new StreamReader(errorResponse.GetResponseStream());
                    string error = await reader.ReadToEndAsync();
                    Debug.WriteLine("OAuth Error: " + error);
                }
                return null;
            }
        }

        private string GenerateRandomBase64Url(int length)
        {
            using var rng = RandomNumberGenerator.Create();
            byte[] bytes = new byte[length];
            rng.GetBytes(bytes);
            return Base64UrlEncodeNoPadding(bytes);
        }

        private byte[] SHA256Hash(string input)
        {
            using var sha256 = SHA256.Create();
            return sha256.ComputeHash(Encoding.ASCII.GetBytes(input));
        }

        private string Base64UrlEncodeNoPadding(byte[] input)
        {
            return Convert.ToBase64String(input).Replace("+", "-").Replace("/", "_").Replace("=", "");
        }
    }
}
