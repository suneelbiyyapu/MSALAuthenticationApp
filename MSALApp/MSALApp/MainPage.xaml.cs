using MSALApp.PageModels;
using Xamarin.Forms;

namespace MSALApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            this.BindingContext = new MainPageModel();
        }
    }
}
