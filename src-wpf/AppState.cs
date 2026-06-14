using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using Brush = System.Windows.Media.Brush;
using Color = System.Windows.Media.Color;
using SolidColorBrush = System.Windows.Media.SolidColorBrush;

namespace ValorantUnlocker
{
    /// <summary>
    /// Trạng thái dùng chung toàn app (thay cho phần "Form1.cs" cũ): giữ engine, nhật ký,
    /// và trạng thái hiển thị. Các trang WPF bind thẳng vào đây. Marshal mọi cập nhật về UI thread.
    /// </summary>
    internal sealed class AppState : INotifyPropertyChanged
    {
        public static AppState Current { get; } = new();

        public UnlockerEngine Engine { get; }
        public GameMonitorService Monitor { get; }
        public ObservableCollection<string> Logs { get; } = new();

        /// <summary>MainWindow gán hàm điều hướng cho các trang public.</summary>
        public Action<Type>? Navigate;

        /// <summary>(nội dung, là lỗi?) — MainWindow hứng để hiện hộp thoại lỗi / bong bóng cảnh báo.</summary>
        public event Action<string, bool>? AlertRequested;

        private string _statusText = "Sẵn sàng";
        public string StatusText { get => _statusText; private set { _statusText = value; OnChanged(nameof(StatusText)); } }

        private string _descText = "Public build: chỉ xoá/khôi phục logo VNG cục bộ. Premium/key/cloud không nằm trong mã nguồn này.";
        public string DescText { get => _descText; private set { _descText = value; OnChanged(nameof(DescText)); } }

        private Brush _statusBrush = new SolidColorBrush(Color.FromRgb(46, 204, 113));
        public Brush StatusBrush { get => _statusBrush; private set { _statusBrush = value; OnChanged(nameof(StatusBrush)); } }

        private Brush _statusBackgroundBrush = new SolidColorBrush(Color.FromArgb(68, 46, 204, 113));
        public Brush StatusBackgroundBrush { get => _statusBackgroundBrush; private set { _statusBackgroundBrush = value; OnChanged(nameof(StatusBackgroundBrush)); } }

        private Brush _statusBorderBrush = new SolidColorBrush(Color.FromArgb(150, 46, 204, 113));
        public Brush StatusBorderBrush { get => _statusBorderBrush; private set { _statusBorderBrush = value; OnChanged(nameof(StatusBorderBrush)); } }

        private AppState()
        {
            Engine = new UnlockerEngine(Log);
            Monitor = new GameMonitorService(Engine, this);
            ResolvePaths();
        }

        private void ResolvePaths()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            Log($"Thư mục khởi chạy: {baseDir}");
            Engine.ResolveDataFolders(baseDir);

            Log("[TỰ ĐỘNG] Bắt đầu tự động dò quét thư mục game...");
            string detected = Engine.AutoDetectGamePath();
            if (!string.IsNullOrEmpty(detected) && Directory.Exists(detected))
            {
                Engine.GamePath = detected;
                Log($"Đường dẫn thư mục game đang sử dụng: {Engine.GamePath}");
                Engine.RestoreVngFiles(silent: true);
            }
            else
            {
                Log("[LỖI] Không tự tìm được thư mục cài đặt VALORANT.");
                Log("[LỖI] Hãy bấm \"Chọn thư mục game\" để chỉ đường dẫn thủ công.");
                SetStatus("Thiếu đường dẫn game", Color.FromRgb(231, 76, 60));
            }
        }

        /// <summary>Người dùng chọn thủ công thư mục game; trả về true nếu hợp lệ.</summary>
        public bool SetGameFolderFromSelection(string selected)
        {
            string paks = ResolvePaksPath(selected);
            if (string.IsNullOrEmpty(paks) || !Directory.Exists(paks)) return false;
            Engine.GamePath = paks;
            Log($"[THỦ CÔNG] Đã đặt đường dẫn game thủ công: {Engine.GamePath}");
            Engine.RestoreVngFiles(silent: true);
            SetStatus("Sẵn sàng", Color.FromRgb(46, 204, 113));
            return true;
        }

        /// <summary>Suy ra thư mục Paks từ thư mục người dùng chọn.</summary>
        private static string ResolvePaksPath(string selected)
        {
            if (selected.EndsWith("Paks", StringComparison.OrdinalIgnoreCase))
                return selected;

            string[] rels =
            {
                Path.Combine("live", "ShooterGame", "Content", "Paks"),
                Path.Combine("ShooterGame", "Content", "Paks"),
                Path.Combine("Content", "Paks")
            };
            foreach (var rel in rels)
            {
                string candidate = Path.Combine(selected, rel);
                if (Directory.Exists(candidate)) return candidate;
            }
            try
            {
                if (Directory.GetFiles(selected, "*.pak").Length > 0) return selected;
            }
            catch { }
            return "";
        }

        public void SetStatus(string text, Color color) =>
            Dispatch(() =>
            {
                StatusText = text;
                StatusBrush = new SolidColorBrush(color);
                StatusBackgroundBrush = new SolidColorBrush(Color.FromArgb(68, color.R, color.G, color.B));
                StatusBorderBrush = new SolidColorBrush(Color.FromArgb(150, color.R, color.G, color.B));
            });

        public void SetDesc(string text) => Dispatch(() => DescText = text);

        public void Log(string message)
        {
            Dispatch(() =>
            {
                Logs.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
                if (message.Contains("[LỖI]")) AlertRequested?.Invoke(Clean(message), true);
                else if (message.Contains("[CẢNH BÁO]")) AlertRequested?.Invoke(Clean(message), false);
            });
        }

        private static string Clean(string m) => m.Replace("[LỖI]", "").Replace("[CẢNH BÁO]", "").Trim();

        private static void Dispatch(Action a)
        {
            var app = System.Windows.Application.Current;
            if (app == null || app.Dispatcher.CheckAccess()) a();
            else app.Dispatcher.Invoke(a);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
