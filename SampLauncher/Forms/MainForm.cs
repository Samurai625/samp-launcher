using System;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;
using System.IO;
using SAMPLauncher.Logic;
using System.Threading.Tasks;
#nullable enable

namespace SampLauncher.Forms
{
    public class MainForm : Form
    {
        private WebView2 webView;

        public MainForm()
        {
            this.Text = "SAMP Launcher";
            this.Width = 1600;
            this.Height = 800;
            MaximizeBox = false;

            webView = new WebView2
            {
                Dock = DockStyle.Fill
            };

            this.Controls.Add(webView);
            this.Load += MainForm_Load;
        }

        private const string VersionFileUrl = "https://raw.githubusercontent.com/Samurai625/samp-launcher/main/version.txt";
        private const string DownloadUrl = "https://github.com/Samurai625/samp-launcher/releases/download/v.1.0.0/GTA.San.Andreas.zip";
        private string gamePath = @"G:\SampLauncher\Game";
        private bool isDownloading = false;

        private async void MainForm_Load(object? sender, EventArgs e)
        {
            await webView.EnsureCoreWebView2Async(null);
            webView.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;


            webView.CoreWebView2.NavigationStarting += CoreWebView2_NavigationStarting;
            string htmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "index.html");

            string url = new Uri(htmlPath).AbsoluteUri;

            webView.Source = new Uri(url);
        }

        private void CoreWebView2_NavigationStarting(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs e)
        {
            string uri = e.Uri;

            if (uri.StartsWith("http://") || uri.StartsWith("https://"))
            {
                e.Cancel = true;
                Process.Start(new ProcessStartInfo(uri) { UseShellExecute = true });
            }
        }
        private async void CoreWebView2_WebMessageReceived(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                var doc = System.Text.Json.JsonDocument.Parse(e.WebMessageAsJson);
                string? command = doc.RootElement.GetProperty("command").GetString();

                if (command == "playButtonClicked")
                {
                    string? nickname = doc.RootElement.GetProperty("nickname").GetString();
                    string? server = doc.RootElement.GetProperty("server").GetString();

                    await HandlePlayClick(nickname!, server!);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка обробки повідомлення: " + ex.Message);
            }
        }

        private async Task HandlePlayClick(string nickname, string server)
        {
            if (isDownloading) return;

            if (string.IsNullOrWhiteSpace(nickname) || string.IsNullOrWhiteSpace(server))
            {
                MessageBox.Show("Будь ласка, введіть нікнейм та виберіть сервер.");
                return;
            }
            string sampExe = Path.Combine(gamePath, "samp.exe");

            var checkProgress = new Progress<int>(p =>
            {
                SendProgressToWeb(p, p == 0 ? "Потрібно оновити гру" : "Перевірка файлів...");
            });

            bool valid = await FileVerifier.CheckFilesAsync(gamePath, checkProgress);

            if (!valid)
            {
                SendProgressToWeb(0, "Завантаження оновлення...");
                isDownloading = true;

                string zipPath = Path.Combine(Path.GetTempPath(), "GTA.San.Andreas.zip");
                var d1progress = new Progress<int>(p =>
                {
                    SendProgressToWeb(p, $"Завантаження:");
                });

                await Downloader.DownloadFileAsync(DownloadUrl, zipPath, d1progress);

                SendProgressToWeb(100, "Розпакування файлів...");
                await Unpacker.ExtractZipAsync(zipPath, gamePath);
                File.Delete(zipPath);

                File.WriteAllText(Path.Combine(gamePath, "version.txt"), "1.0.0");
                FileVerifier.SaveFileList(gamePath);
                isDownloading = false;
                SendProgressToWeb(100, "Готово до гри!");
            }
            else
            {
                SendProgressToWeb(100, "Готово до гри!");
            }

            GameManager.LaunchGame(gamePath, nickname, server);
        }

        private void SendProgressToWeb(int progress, string message)
        {
            string json = System.Text.Json.JsonSerializer.Serialize(new
            {
                type = "progress",
                percent = progress,
                message
            });

            webView.CoreWebView2.PostWebMessageAsJson(json);
        }
    }
}
