using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Security.Cryptography;
using System.Windows.Controls;
using System.Diagnostics;

namespace music_sync_frontend.Views
{
    public partial class Authentication : Window
    {
        string clientID = System.Configuration.ConfigurationManager.AppSettings["CliendId"];
        const string authorizationEndpoint = "https://accounts.spotify.com/authorize";

        public Authentication()
        {
            InitializeComponent();
        }

        private async void button_Click(object sender, RoutedEventArgs e)
        {
            string state = randomDataBase64url(32);
            string code_verifier = randomDataBase64url(32);
            string code_challenge = base64urlencodeNoPadding(sha256(code_verifier));
            const string code_challenge_method = "S256";

            string redirectURI = string.Format("http://{0}:{1}/", IPAddress.Loopback, System.Configuration.ConfigurationManager.AppSettings["RedirectUriPort"]);
            output("redirect URI: " + redirectURI);

            var http = new HttpListener();
            http.Prefixes.Add(redirectURI);
            output("Listening..");
            http.Start();

            // Creates the OAuth 2.0 authorization request
            string authorizationRequest = string.Format("{0}?response_type=code&redirect_uri={1}&client_id={2}&code_challenge={3}&code_challenge_method={4}",
                authorizationEndpoint,
                redirectURI,
                clientID,
                code_challenge,
                code_challenge_method);

            // Opens request in the browser
            var psi = new ProcessStartInfo
            {
                FileName = authorizationRequest,
                UseShellExecute = true
            };

            Process.Start(psi);

            var context = await http.GetContextAsync();

            // Brings this app back to the foreground
            this.Activate();

            // Sends an HTTP response to the browser
            var response = context.Response;
            string responseString = string.Format("<html><body>Please return to the app.</body></html>");
            var buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            var responseOutput = response.OutputStream;
            Task responseTask = responseOutput.WriteAsync(buffer, 0, buffer.Length).ContinueWith((task) =>
            {
                responseOutput.Close();
                http.Stop();
                Console.WriteLine("HTTP server stopped.");
            });

            if (context.Request.QueryString.Get("code") == null)
            {
                output("Malformed authorization response. " + context.Request.QueryString);
                return;
            }

            var code = context.Request.QueryString.Get("code");

            output("Authorization code: " + code);

            performCodeExchange(code, code_verifier, redirectURI);
        }

        async void performCodeExchange(string code, string code_verifier, string redirectURI)
        {
            output("Exchanging code for tokens...");

            string tokenRequestURI = "https://accounts.spotify.com/api/token";
            string tokenRequestBody = string.Format("code={0}&redirect_uri={1}&client_id={2}&code_verifier={3}&grant_type=authorization_code",
                code,
                redirectURI,
                clientID,
                code_verifier
                );

            HttpWebRequest tokenRequest = (HttpWebRequest)WebRequest.Create(tokenRequestURI);
            tokenRequest.Method = "POST";
            tokenRequest.ContentType = "application/x-www-form-urlencoded";
            byte[] _byteVersion = Encoding.ASCII.GetBytes(tokenRequestBody);
            tokenRequest.ContentLength = _byteVersion.Length;
            Stream stream = tokenRequest.GetRequestStream();
            await stream.WriteAsync(_byteVersion, 0, _byteVersion.Length);
            stream.Close();

            try
            {
                WebResponse tokenResponse = await tokenRequest.GetResponseAsync();
                using (StreamReader reader = new StreamReader(tokenResponse.GetResponseStream()))
                {
                    string responseText = await reader.ReadToEndAsync();
                    output(responseText);

                    Dictionary<string, string> tokenEndpointDecoded = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseText);


                    string access_token = tokenEndpointDecoded["access_token"];
                    output("Access token: " + access_token);
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    var response = ex.Response as HttpWebResponse;
                    if (response != null)
                    {
                        output("HTTP: " + response.StatusCode);
                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        {
                            // reads response body
                            string responseText = await reader.ReadToEndAsync();
                            output(responseText);
                        }
                    }

                }
            }
        }


        public void output(string output)
        {
            TextBox textBoxOutput = (TextBox)this.FindName("textBoxOutput");
            textBoxOutput.Text = textBoxOutput.Text + output + Environment.NewLine;
            Console.WriteLine(output);
        }

        public static string randomDataBase64url(uint length)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] bytes = new byte[length];
            rng.GetBytes(bytes);
            return base64urlencodeNoPadding(bytes);
        }

        public static byte[] sha256(string inputStirng)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(inputStirng);
            SHA256Managed sha256 = new SHA256Managed();
            return sha256.ComputeHash(bytes);
        }

        public static string base64urlencodeNoPadding(byte[] buffer)
        {
            string base64 = Convert.ToBase64String(buffer);

            base64 = base64.Replace("+", "-");
            base64 = base64.Replace("/", "_");
            base64 = base64.Replace("=", "");

            return base64;
        }

    }
}
