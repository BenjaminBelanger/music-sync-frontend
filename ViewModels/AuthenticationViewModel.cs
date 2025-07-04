
using music_sync_frontend.Helpers;
using music_sync_frontend.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace music_sync_frontend.ViewModels
{
    public class AuthenticationViewModel : INotifyPropertyChanged
    {
        private readonly ISpotifyAuthService _spotifyAuthService;

        public ICommand AuthenticateCommand { get; }

        private string _output;
        public string Output
        {
            get => _output;
            set { _output = value; OnPropertyChanged(); }
        }

        public AuthenticationViewModel(ISpotifyAuthService spotifyAuthService)
        {
            _spotifyAuthService = spotifyAuthService;
            AuthenticateCommand = new RelayCommand(async () => await AuthenticateAsync());
        }

        private async Task AuthenticateAsync()
        {
            Output = "Starting authentication...";
            var token = await _spotifyAuthService.AuthenticateAsync();
            Output = token != null ? $"Access token: {token}" : "Authentication failed.";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}
