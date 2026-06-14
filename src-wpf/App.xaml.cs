using System.Threading;
using System.Windows;
using ValorantUnlocker.Views;

namespace ValorantUnlocker
{
    /// <summary>
    /// Điểm vào ứng dụng. Bảo đảm chỉ 1 instance; instance thứ hai sẽ "đánh thức" cửa sổ đang chạy
    /// (hiện lên từ khay) qua một EventWaitHandle có tên, rồi tự thoát.
    /// </summary>
    public partial class App
    {
        private const string MutexName = @"Global\ValoUnlock_SingleInstance_8F21";
        private const string ShowEventName = @"Global\ValoUnlock_ShowMe_8F21";

        private Mutex? _mutex;
        private EventWaitHandle? _showEvent;
        private MainWindow? _mainWindow;

        protected override void OnStartup(StartupEventArgs e)
        {
            _mutex = new Mutex(true, MutexName, out bool createdNew);
            if (!createdNew)
            {
                // Đã có instance chạy -> ra hiệu cho nó hiện lên rồi thoát.
                try { EventWaitHandle.OpenExisting(ShowEventName).Set(); } catch { /* không mở được thì thôi */ }
                Shutdown();
                return;
            }

            base.OnStartup(e);

            // Áp theme WPF-UI: chế độ Tối + nền Mica + LẤY MÀU NHẤN THẬT của Windows (accent).
            // Thiếu bước này thì chữ về đen mặc định, nút dùng accent giả, icon Fluent không hiện đúng.
            Wpf.Ui.Appearance.ApplicationThemeManager.Apply(
                Wpf.Ui.Appearance.ApplicationTheme.Dark,
                Wpf.Ui.Controls.WindowBackdropType.Mica,
                updateAccent: true);

            // Lắng nghe tín hiệu "hiện cửa sổ" từ instance thứ hai.
            _showEvent = new EventWaitHandle(false, EventResetMode.AutoReset, ShowEventName);
            var listener = new Thread(() =>
            {
                while (true)
                {
                    _showEvent.WaitOne();
                    Dispatcher.Invoke(() => _mainWindow?.ShowFromTray());
                }
            })
            { IsBackground = true };
            listener.Start();

            DispatcherUnhandledException += (_, ex) =>
            {
                try { System.IO.File.WriteAllText(System.IO.Path.Combine(System.IO.Path.GetTempPath(), "valounlock_crash.log"), ex.Exception.ToString()); } catch { }
            };

            try
            {
                _mainWindow = new MainWindow();
                MainWindow = _mainWindow;
                _mainWindow.Show();
            }
            catch (Exception ex)
            {
                try { System.IO.File.WriteAllText(System.IO.Path.Combine(System.IO.Path.GetTempPath(), "valounlock_crash.log"), ex.ToString()); } catch { }
                throw;
            }
        }
    }
}
