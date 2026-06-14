using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Color = System.Windows.Media.Color;

namespace ValorantUnlocker
{
    /// <summary>
    /// Public/free monitor: game mở -> xoá logo VNG cục bộ; game đóng -> khôi phục file gốc.
    /// Không chứa license, private backend, private cloud sync, máu đỏ, xác chết hoặc logic premium.
    /// </summary>
    internal sealed class GameMonitorService
    {
        private readonly UnlockerEngine _engine;
        private readonly AppState _app;

        private CancellationTokenSource? _cts;
        private bool _hasInjected;

        public bool IsMonitoring { get; private set; }

        private static readonly Color Ready = Color.FromRgb(46, 204, 113);
        private static readonly Color Working = Color.FromRgb(241, 196, 15);
        private static readonly Color Playing = Color.FromRgb(52, 152, 219);
        private static readonly Color Paused = Color.FromRgb(149, 165, 166);

        public GameMonitorService(UnlockerEngine engine, AppState app)
        {
            _engine = engine;
            _app = app;
        }

        public void Start()
        {
            if (IsMonitoring) return;
            IsMonitoring = true;

            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            _app.Log("Bắt đầu theo dõi game.");
            _app.SetStatus("Sẵn sàng", Ready);
            _app.SetDesc("Public build: vào game sẽ xoá logo VNG; thoát game sẽ khôi phục file gốc.");

            Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        bool running =
                            Process.GetProcessesByName("VALORANT-Win64-Shipping").Length > 0 ||
                            Process.GetProcessesByName("VALORANT").Length > 0;

                        if (running && !_hasInjected)
                        {
                            _app.SetStatus("Đang mở khóa", Working);
                            _app.SetDesc("Đang sao lưu và xoá logo VNG...");
                            _engine.InjectPublicFiles();
                            _hasInjected = true;
                            _app.SetStatus("Đang chơi", Playing);
                            _app.SetDesc("Đã xoá logo VNG. Bản public không bật máu đỏ hoặc xác chết.");
                        }
                        else if (!running && _hasInjected)
                        {
                            _app.Log("Game đã đóng — đang khôi phục file VNG gốc...");
                            _app.SetStatus("Đang khôi phục", Working);
                            _app.SetDesc("Đang trả lại file gốc của VNG...");
                            _engine.RestoreVngFiles(silent: false);
                            _hasInjected = false;
                            _app.SetStatus("Sẵn sàng", Ready);
                            _app.SetDesc("Public build: vào game sẽ xoá logo VNG; thoát game sẽ khôi phục file gốc.");
                        }
                    }
                    catch
                    {
                        // Bỏ qua lỗi tiến trình/tệp tạm thời; lần quét sau sẽ thử lại.
                    }

                    await Task.Delay(100, token);
                }
            }, token);
        }

        public void Stop()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
            }

            IsMonitoring = false;
            _app.Log("Đã tạm dừng theo dõi.");
            _app.SetStatus("Đã tạm dừng", Paused);
            _app.SetDesc("Đang tắt theo dõi. Bấm \"Bật theo dõi\" để tiếp tục.");
        }
    }
}
