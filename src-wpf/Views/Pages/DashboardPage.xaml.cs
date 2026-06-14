using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Forms = System.Windows.Forms;

namespace ValorantUnlocker.Views.Pages
{
    public partial class DashboardPage : Page
    {
        public DashboardPage()
        {
            InitializeComponent();
            DataContext = AppState.Current;
            AppState.Current.Logs.CollectionChanged += Logs_CollectionChanged;

            Loaded += (_, _) => SyncMonitorButton();
        }

        private void Logs_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
            => LogScroll.ScrollToEnd();

        private void BtnMonitor_Click(object sender, RoutedEventArgs e)
        {
            var mon = AppState.Current.Monitor;
            if (mon.IsMonitoring) mon.Stop();
            else mon.Start();
            SyncMonitorButton();
        }

        private void SyncMonitorButton()
        {
            bool on = AppState.Current.Monitor.IsMonitoring;
            BtnMonitor.Content = on ? "Tạm dừng" : "Bật theo dõi";
        }

        private void BtnRestore_Click(object sender, RoutedEventArgs e)
        {
            AppState.Current.Log("Đang bắt buộc khôi phục lại các tệp VNG gốc...");
            Task.Run(() => AppState.Current.Engine.RestoreVngFiles(false));
        }

        private void BtnChoose_Click(object sender, RoutedEventArgs e)
        {
            using var dlg = new Forms.FolderBrowserDialog
            {
                Description = "Chọn thư mục cài đặt VALORANT (thư mục game hoặc thư mục Paks)",
                UseDescriptionForTitle = true
            };
            if (dlg.ShowDialog() != Forms.DialogResult.OK) return;

            if (!AppState.Current.SetGameFolderFromSelection(dlg.SelectedPath))
            {
                System.Windows.MessageBox.Show(
                    "Thư mục được chọn không phải thư mục VALORANT hoặc không chứa thư mục Paks hợp lệ!",
                    "Lỗi đường dẫn", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }
    }
}
