using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Security.Cryptography;
using System.Windows.Controls;
using System.Diagnostics;
using music_sync_frontend.ViewModels;
using music_sync_frontend.Services;

namespace music_sync_frontend.Views
{
    public partial class Authentication : Window
    {
        public Authentication()
        {
            InitializeComponent();

            DataContext = new AuthenticationViewModel(new SpotifyAuthService());
        }
    }
}
