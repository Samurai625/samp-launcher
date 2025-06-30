    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.Json;
    using System.Threading.Tasks;
    #nullable enable

    namespace SAMPLauncher.Logic
    {
        public static class FileVerifier
        {
            public static void SaveFileList(string gamePath)
            {
                var files = Directory.GetFiles(gamePath, "*.*", SearchOption.AllDirectories);
                var fileDict = new Dictionary<string, long>();

                foreach (var file in files)
                {
                    string relativePath = Path.GetRelativePath(gamePath, file).Replace("\\", "/");
                    long size = new FileInfo(file).Length;
                    fileDict[relativePath] = size;
                }

                string json = JsonSerializer.Serialize(fileDict, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(Path.Combine(gamePath, "filelist.json"), json);
            }

            public static async Task<bool> CheckFilesAsync(string gamePath, IProgress<int>? progress = null)
            {
                string fileListPath = Path.Combine(gamePath, "filelist.json");
                if (!File.Exists(fileListPath))
                    return false;

                var json = File.ReadAllText(fileListPath);
                var expectedFiles = JsonSerializer.Deserialize<Dictionary<string, long>>(json);

                if (expectedFiles == null)
                    return false;

                int total = expectedFiles.Count;
                int current = 0;

                foreach (var kvp in expectedFiles)
                {
                    string fullPath = Path.Combine(gamePath, kvp.Key.Replace("/", Path.DirectorySeparatorChar.ToString()));
                    if (!File.Exists(fullPath))
                        return false;

                    long actualSize = new FileInfo(fullPath).Length;
                    if (actualSize != kvp.Value)
                        return false;

                    current++;
                    int percent = (int)((double)current / total * 100);
                    progress?.Report(percent);
                    await Task.Delay(1);
                }

                return true;
            }
        }
    }
