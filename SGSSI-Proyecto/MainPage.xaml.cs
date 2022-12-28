using SGSSI_Proyecto.Views;
using Windows.UI.Xaml.Controls;

namespace SGSSI_Proyecto
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            ContentFrame.Navigate(typeof(HashView));
        }
    }
}
