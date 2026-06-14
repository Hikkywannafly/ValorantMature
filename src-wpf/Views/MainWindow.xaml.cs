using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ValorantUnlocker.Views.Pages;
using Wpf.Ui.Controls;
using Drawing = System.Drawing;
using Forms = System.Windows.Forms;

namespace ValorantUnlocker.Views
{
    /// <summary>
    /// Cửa sổ chính (FluentWindow + NavigationView). Quản lý tray, ẩn xuống khay khi đóng,
    /// và "đánh thức" khi instance thứ hai ra hiệu.
    /// </summary>
    public partial class MainWindow : FluentWindow
    {
        private Forms.NotifyIcon? _tray;
        private bool _reallyExit;

        public MainWindow()
        {
            InitializeComponent();
            // Áp theme + nền Mica TRỰC TIẾP lên cửa sổ này (bền hơn chỉ apply ở cấp App;
            // bảo đảm brush chữ/nền của Win11 luôn đúng, không bị về đen).
            Wpf.Ui.Appearance.SystemThemeWatcher.Watch(this);
            InitTray();
            AppState.Current.AlertRequested += OnAlertRequested;
            this.Closing += MainWindow_Closing;
            // NavigationView cần template dựng xong mới điều hướng được (tránh NRE).
            this.Loaded += OnLoaded;

            // NGUYÊN NHÂN GỐC của lỗi không cuộn được: NavigationView (WPF-UI) đánh dấu sự kiện lăn chuột
            // là Handled khi nó tunnel xuống, nên ScrollViewer bên trong không bao giờ tự cuộn.
            // Giải pháp chắc chắn: bắt ở CỬA SỔ (handledEventsToo=true), tìm ScrollViewer ngay dưới con trỏ
            // rồi tự cuộn. Cấp cửa sổ là tổ tiên của mọi thứ nên không gì chặn được.
            AddHandler(UIElement.PreviewMouseWheelEvent, new MouseWheelEventHandler(OnGlobalMouseWheel), true);
        }

        /// <summary>Cuộn ScrollViewer nằm dưới con trỏ chuột khi lăn — bù cho việc NavigationView nuốt event.</summary>
        private void OnGlobalMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.OriginalSource is not DependencyObject d) return;

            var sv = FindScrollableParent(d, e.Delta);
            if (sv == null) return;

            sv.ScrollToVerticalOffset(sv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        /// <summary>Đi ngược cây UI từ phần tử đang trỏ tới để tìm ScrollViewer còn cuộn được.</summary>
        private static ScrollViewer? FindScrollableParent(DependencyObject? node, int delta)
        {
            while (node != null)
            {
                if (node is ScrollViewer sv && CanScroll(sv, delta)) return sv;
                node = GetParent(node);
            }
            return null;
        }

        private static bool CanScroll(ScrollViewer sv, int delta)
        {
            if (sv.ScrollableHeight <= 0) return false;
            return delta < 0
                ? sv.VerticalOffset < sv.ScrollableHeight
                : sv.VerticalOffset > 0;
        }

        private static DependencyObject? GetParent(DependencyObject node)
        {
            if (node is Visual || node is System.Windows.Media.Media3D.Visual3D)
                return VisualTreeHelper.GetParent(node) ?? LogicalTreeHelper.GetParent(node);

            return LogicalTreeHelper.GetParent(node);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Cho các trang public chuyển tab khi cần.
            AppState.Current.Navigate = t => RootNavigation.Navigate(t);

            AppState.Current.Monitor.Start();
            RootNavigation.Navigate(typeof(DashboardPage));
        }

        private void InitTray()
        {
            var menu = new Forms.ContextMenuStrip();
            menu.Items.Add("Mở bảng điều khiển", null, (_, _) => ShowFromTray());
            menu.Items.Add("Khôi phục VNG gốc", null, (_, _) => Task.Run(() => AppState.Current.Engine.RestoreVngFiles(false)));
            menu.Items.Add(new Forms.ToolStripSeparator());
            menu.Items.Add("Thoát hoàn toàn", null, (_, _) => ExitApp());

            _tray = new Forms.NotifyIcon
            {
                Text = "VALORANT Mature Unlocker",
                Visible = true,
                ContextMenuStrip = menu,
                Icon = LoadTrayIcon()
            };
            _tray.DoubleClick += (_, _) => ShowFromTray();
        }

        private static Drawing.Icon LoadTrayIcon()
        {
            try
            {
                string dir = AppDomain.CurrentDomain.BaseDirectory;
                for (int i = 0; i < 5 && !string.IsNullOrEmpty(dir); i++)
                {
                    string p = Path.Combine(dir, "icon", "icon.ico");
                    if (File.Exists(p)) return new Drawing.Icon(p);
                    dir = Path.GetDirectoryName(dir.TrimEnd('\\')) ?? "";
                }
            }
            catch { /* dùng icon mặc định */ }
            return Drawing.SystemIcons.Application;
        }

        public void ShowFromTray()
        {
            this.Show();
            this.WindowState = WindowState.Normal;
            this.Activate();
            this.Topmost = true;
            this.Topmost = false;
        }

        private void MainWindow_Closing(object? sender, CancelEventArgs e)
        {
            if (_reallyExit) return;
            e.Cancel = true;
            this.Hide();
            _tray?.ShowBalloonTip(3000, "Vẫn đang chạy ẩn",
                "Công cụ vẫn chạy dưới khay hệ thống để tự động theo dõi game.", Forms.ToolTipIcon.Info);
        }

        private void OnAlertRequested(string content, bool isError)
        {
            Dispatcher.Invoke(() =>
            {
                if (isError)
                    System.Windows.MessageBox.Show(content, "Đã xảy ra lỗi",
                        System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                else
                    _tray?.ShowBalloonTip(4000, "Cảnh báo", content, Forms.ToolTipIcon.Warning);
            });
        }

        private void ExitApp()
        {
            _reallyExit = true;
            try { AppState.Current.Monitor.Stop(); } catch { /* vẫn thoát */ }
            try { AppState.Current.Engine.RestoreVngFiles(true); } catch { /* vẫn thoát */ }
            if (_tray != null) { _tray.Visible = false; _tray.Dispose(); }
            System.Windows.Application.Current.Shutdown();
        }
    }
}
