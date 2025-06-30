using System.Diagnostics;
using System.IO;


namespace SAMPLauncher.Logic
{
    public static class GameManager
    {
        public static void LaunchGame(string path, string nickname, string server)
        {
            string exePath = Path.Combine(path, "samp.exe");

            if (!File.Exists(exePath))
                throw new FileNotFoundException("samp.exe не знайдено.");

            Process.Start(new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = $"{server} -n{nickname}",
                WorkingDirectory = path,
                UseShellExecute = true
            });
        }
    }
}
