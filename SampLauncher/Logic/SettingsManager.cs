using System;
using System.IO;
using System.Text.Json;

public static class SettingsManager
{
    private static readonly string SettingsPath = "settings.json";

    public class Settings
    {
        public string GamePath { get; set; } = "";
    }

    public static Settings Load()
    {
        if (File.Exists(SettingsPath))
        {
            string json = File.ReadAllText(SettingsPath);
            return JsonSerializer.Deserialize<Settings>(json) ?? new Settings();
        }

        return new Settings();
    }

    public static void Save(Settings settings)
    {
        string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(SettingsPath, json);
    }
}
