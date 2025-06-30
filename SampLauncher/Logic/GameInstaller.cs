using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SAMPLauncher.Logic
{
    public static class GameInstaller
    {
        public static async Task InstallAsync(string url, string gamePath, Action<int> onProgress)
        {
            try
            {
                Directory.CreateDirectory(gamePath);
                string zipPath = Path.Combine(gamePath, "game.zip");

                var progress = new Progress<int>(onProgress);
                await Downloader.DownloadFileAsync(url, zipPath, progress);
                await Unpacker.ExtractZipAsync(zipPath, gamePath);
                File.Delete(zipPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка встановлення гри: " + ex.Message);
            }
        }
    }
}
