using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
#nullable enable

namespace SAMPLauncher.Logic
{
    public static class Downloader
    {
        public static async Task DownloadFileAsync(string url, string filePath, IProgress<int>? progress = null)
        {
            using var httpClient = new HttpClient();
            using var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

            response.EnsureSuccessStatusCode();
            var totalBytes = response.Content.Headers.ContentLength ?? -1L;
            var canReport = totalBytes != -1 && progress != null;

            using var contentStream = await response.Content.ReadAsStreamAsync();
            using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);

            var buffer = new byte[8192];
            long totalRead = 0;
            int read;
            while ((read = await contentStream.ReadAsync(buffer.AsMemory(0, buffer.Length))) > 0)
            {
                await fileStream.WriteAsync(buffer.AsMemory(0, read));
                totalRead += read;
                if (canReport)
                    progress!.Report((int)(totalRead * 100 / totalBytes));
            }
        }
    }
}
