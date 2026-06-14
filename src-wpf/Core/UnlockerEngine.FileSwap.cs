using System;
using System.IO;

namespace ValorantUnlocker
{
    /// <summary>Public/free file swap: sao lưu, xoá và khôi phục các file logo VNG.</summary>
    internal partial class UnlockerEngine
    {
        /// <summary>Xoá logo VNG sau khi đã sao lưu file gốc. Không gọi license, backend hoặc cloud sync.</summary>
        public void InjectPublicFiles()
        {
            if (!HasValidGamePath) return;

            try
            {
                foreach (var file in VngLogoFiles)
                {
                    string sourcePath = Path.Combine(GamePath, file);
                    if (!File.Exists(sourcePath)) continue;

                    try
                    {
                        if (!Directory.Exists(BackupFolder))
                            Directory.CreateDirectory(BackupFolder);
                        File.Copy(sourcePath, Path.Combine(BackupFolder, file), true);
                    }
                    catch (Exception ex)
                    {
                        log($"[LỖI] Không thể sao lưu file {file}: {ex.Message}");
                    }

                    DeleteFileSafe(sourcePath);
                }

                log("Đã xoá logo VNG bằng chế độ public.");
            }
            catch (Exception ex)
            {
                log($"[LỖI] Xoá logo VNG thất bại: {ex.Message}");
            }
        }

        /// <summary>Khôi phục logo VNG gốc từ backup.</summary>
        public void RestoreVngFiles(bool silent = false)
        {
            if (!HasValidGamePath) return;

            try
            {
                foreach (var file in VngLogoFiles)
                {
                    string backupPath = Path.Combine(BackupFolder, file);
                    if (File.Exists(backupPath))
                        CopyFileSafe(backupPath, Path.Combine(GamePath, file));
                }

                if (!silent)
                    log("Đã khôi phục các tệp VNG gốc thành công!");
            }
            catch (Exception ex)
            {
                log($"[LỖI] Khôi phục tệp thất bại: {ex.Message}");
            }
        }

        /// <summary>Gỡ bỏ thuộc tính Read-Only khỏi toàn bộ các file .ini cấu hình.</summary>
        public void UnlockGameUserSettings()
        {
            RemoveReadOnlyFromConfigs(verbose: true);
        }

        private void RemoveReadOnlyFromConfigs(bool verbose)
        {
            try
            {
                string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string configRoot = Path.Combine(localAppData, "VALORANT", "Saved", "Config");
                if (!Directory.Exists(configRoot)) return;

                var iniFiles = Directory.GetFiles(configRoot, "*.ini", SearchOption.AllDirectories);
                foreach (var file in iniFiles)
                {
                    var attributes = File.GetAttributes(file);
                    if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    {
                        File.SetAttributes(file, attributes & ~FileAttributes.ReadOnly);
                        if (verbose)
                            log($"[BẢO TRÌ] Đã gỡ bỏ thuộc tính Read-Only khỏi: {Path.GetFileName(file)}");
                    }
                }
            }
            catch (Exception ex)
            {
                log($"[CẢNH BÁO] Không thể gỡ thuộc tính Read-Only khỏi các file cấu hình: {ex.Message}");
            }
        }
    }
}
