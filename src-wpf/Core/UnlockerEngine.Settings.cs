using System;
using System.IO;
using System.Text.Json.Nodes;

namespace ValorantUnlocker
{
    /// <summary>Lưu cấu hình public tối thiểu của ứng dụng.</summary>
    internal partial class UnlockerEngine
    {
        public int DataVersion { get; set; } = 1;

        private static string SettingsPath =>
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "ValorantMature",
                "settings.json");

        public void LoadSettings()
        {
            try
            {
                if (!File.Exists(SettingsPath)) return;
                var node = JsonNode.Parse(File.ReadAllText(SettingsPath));
                DataVersion = node?["dataVersion"]?.GetValue<int>() ?? 1;
            }
            catch
            {
                // Hỏng file cấu hình thì dùng mặc định.
            }
        }

        public void SaveSettings()
        {
            try
            {
                string dir = Path.GetDirectoryName(SettingsPath)!;
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                var node = new JsonObject
                {
                    ["dataVersion"] = DataVersion
                };
                File.WriteAllText(SettingsPath, node.ToJsonString());
            }
            catch
            {
                // Không lưu được thì bỏ qua.
            }
        }
    }
}
