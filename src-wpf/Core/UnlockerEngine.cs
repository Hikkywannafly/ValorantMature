using System;
using System.IO;

namespace ValorantUnlocker
{
    /// <summary>
    /// Lõi xử lý public: dò tìm đường dẫn game, sao lưu/xoá/khôi phục logo VNG.
    /// Hoàn toàn tách khỏi giao diện (chỉ ghi log qua callback) để dễ bảo trì và tái sử dụng.
    ///
    /// Lớp được chia thành nhiều file partial theo nhóm chức năng:
    ///   - UnlockerEngine.cs           : lõi, hằng số, helper file dùng chung.
    ///   - UnlockerEngine.Detection.cs : dò tìm đường dẫn game &amp; thư mục data/backup.
    ///   - UnlockerEngine.FileSwap.cs  : xoá/khôi phục logo VNG, gỡ Read-Only cấu hình.
    /// </summary>
    internal partial class UnlockerEngine
    {
        private readonly Action<string> log;

        /// <summary>Thư mục Paks của game đang sử dụng.</summary>
        public string GamePath { get; set; } = "";
        /// <summary>Thư mục sao lưu các tệp VNG gốc.</summary>
        public string BackupFolder { get; private set; } = "";

        // Các tệp logo VNG gốc cần sao lưu / khôi phục.
        private static readonly string[] VngLogoFiles =
        {
            "VNGLogo-WindowsClient.sig",
            "VNGLogo-WindowsClient.pak",
            "VNGLogo-WindowsClient.utoc",
            "VNGLogo-WindowsClient.ucas"
        };

        public UnlockerEngine(Action<string> logger)
        {
            log = logger;
            LoadSettings();
            // UnlockGameUserSettings(); // Tạm thời tắt tính năng trên AppData
        }

        public bool HasValidGamePath => !string.IsNullOrEmpty(GamePath) && Directory.Exists(GamePath);

        private void CopyFileSafe(string source, string destination)
        {
            if (File.Exists(source))
                File.Copy(source, destination, true);
            else
                log($"[LỖI] Thiếu tệp nguồn: {Path.GetFileName(source)}");
        }

        private void DeleteFileSafe(string path)
        {
            if (File.Exists(path))
                File.Delete(path);
        }
    }
}
