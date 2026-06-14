using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace ValorantUnlocker.Views.Pages
{
    public partial class ContactPage : Page
    {
        public ContactPage() => InitializeComponent();

        private void BtnDiscord_Click(object sender, RoutedEventArgs e)
        {
            try { Process.Start(new ProcessStartInfo { FileName = "https://discord.gg/DXX4x5TQRq", UseShellExecute = true }); }
            catch { /* bỏ qua */ }
        }
    }
}
