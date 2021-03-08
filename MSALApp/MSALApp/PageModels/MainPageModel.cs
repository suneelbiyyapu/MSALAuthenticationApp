using MSALApp.Services;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MSALApp.PageModels
{
    public class MainPageModel : INotifyPropertyChanged
    {
        private readonly AuthService _authService;
        private readonly SimpleGraphService _simpleGraphService;

        public event PropertyChangedEventHandler PropertyChanged;

        private bool isSignedIn;
        public bool IsSignedIn 
        { 
            get => isSignedIn; 
            set => SetValue(ref isSignedIn, value); 
        }

        private bool isSigningIn;
        public bool IsSigningIn
        {
            get => isSigningIn;
            set => SetValue(ref isSigningIn, value);
        }

        private string name = string.Empty;
        public string Name
        {
            get => name;
            set => SetValue(ref name, value);
        }

        public ICommand SignInCommand { get; set; }
        public ICommand SignOutCommand { get; set; }

        public MainPageModel()
        {
            _authService = new AuthService();
            _simpleGraphService = new SimpleGraphService();

            SignInCommand = new Command(async()=>await SignInAsync());
            SignOutCommand = new Command(async()=>await SignOutAsync());
        }



        async Task SignInAsync()
        {
            IsSigningIn = true;

            if (await _authService.SignInAsync())
            {
                Name = await _simpleGraphService.GetNameAsync();
                // await _simpleGraphService.GetProfilePicture();
                IsSignedIn = true;
            }

            IsSigningIn = false;
        }

        async Task SignOutAsync()
        {
            if (await _authService.SignOutAsync())
            {
                IsSignedIn = false;
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void SetValue<T>(ref T backingField, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingField, value)) 
                return;
            backingField = value;
            OnPropertyChanged(propertyName);
        }
    }
}
