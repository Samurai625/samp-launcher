using System.IO.Compression;
using System.IO;
using System.Threading.Tasks;

namespace SAMPLauncher.Logic
{
    public static class Unpacker
    {
        public static async Task ExtractZipAsync(string zipPath, string destination)
        {
            await Task.Run(() =>
            {
                ZipFile.ExtractToDirectory(zipPath, destination, overwriteFiles: true);
            });
        }
    }
}
