using System;
using System.IO;

namespace ValorantUnlocker
{
    /// <summary>Dò tìm đường dẫn Paks của game và xác định thư mục backup.</summary>
    internal partial class UnlockerEngine
    {
        /// <summary>Tìm thư mục backup ở thư mục chạy hoặc các thư mục cha.</summary>
        public void ResolveDataFolders(string baseDir)
        {
            string current = baseDir;
            for (int i = 0; i < 4; i++)
            {
                string checkBackup = Path.Combine(current, "backup");
                if (Directory.Exists(checkBackup))
                {
                    BackupFolder = checkBackup;
                    return;
                }
                current = Path.GetDirectoryName(current) ?? "";
                if (string.IsNullOrEmpty(current)) break;
            }

            // Mặc định: tạo đường dẫn ngay cạnh thư mục chạy
            BackupFolder = Path.Combine(baseDir, "backup");
        }

        /// <summary>Tự động dò tìm thư mục Paks của Valorant trên hệ thống.</summary>
        public string AutoDetectGamePath()
        {
            // Bước 1: Quét file cài đặt chính thức của Riot Client
            string riotMetadataPath = @"C:\ProgramData\Riot Games\Metadata\valorant.live\valorant.live.product_settings.yaml";
            if (File.Exists(riotMetadataPath))
            {
                try
                {
                    foreach (var line in File.ReadLines(riotMetadataPath))
                    {
                        if (line.Contains("product_install_full_path:"))
                        {
                            int start = line.IndexOf('"');
                            int end = line.LastIndexOf('"');
                            if (start != -1 && end > start)
                            {
                                string installRoot = line.Substring(start + 1, end - start - 1);
                                installRoot = installRoot.Replace('/', '\\');
                                string paksPath = Path.Combine(installRoot, "ShooterGame", "Content", "Paks");
                                if (Directory.Exists(paksPath))
                                {
                                    log("[TỰ ĐỘNG] Phát hiện đường dẫn thông qua file cấu hình Riot Client Metadata.");
                                    return paksPath;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    log($"[CẢNH BÁO] Quét file cấu hình Riot Client Metadata thất bại: {ex.Message}");
                }
            }

            // Bước 2: Quét tất cả các ổ đĩa theo các cấu trúc thư mục phổ biến (Máy nhà, Quán Net)
            string[] subPaths =
            {
                @"Riot Games\VALORANT\live\ShooterGame\Content\Paks",
                @"Online Games\VALORANT\Data\Riot Games\VALORANT\live\ShooterGame\Content\Paks",
                @"Online Games\VALORANT\live\ShooterGame\Content\Paks",
                @"Games\VALORANT\live\ShooterGame\Content\Paks",
                @"Games\Riot Games\VALORANT\live\ShooterGame\Content\Paks",
                @"VALORANT\live\ShooterGame\Content\Paks"
            };

            try
            {
                foreach (var drive in DriveInfo.GetDrives())
                {
                    if (!drive.IsReady) continue;
                    foreach (var subPath in subPaths)
                    {
                        string candidate = Path.Combine(drive.Name, subPath);
                        if (Directory.Exists(candidate))
                        {
                            log($"[TỰ ĐỘNG] Tìm thấy thư mục game trên ổ {drive.Name.Replace(":\\", "")}: {candidate}");
                            return candidate;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log($"[CẢNH BÁO] Quét các ổ đĩa thất bại: {ex.Message}");
            }

            return "";
        }
    }
}
