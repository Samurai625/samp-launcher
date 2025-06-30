using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using SAMPLauncher.Logic;
using SAMPLauncher.Utils;
using System.Threading.Tasks;
using System.Net.Http;
using SAMPLauncher.Forms.Controls;
#nullable enable


namespace SAMPLauncher.Forms
{
    public class Var1Form : Form
    {
        private TextBox nicknameBox;
        private ComboBox serverBox;
        private ProgressBar progressBar;
        private Button playButton;
        private Label progressLabel;


        private string gamePath = SettingsManager.Load().GamePath;
        private bool isDownloading = false;

        public Var1Form()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();

            Text = "Darmond RP Launcher";
            Size = new Size(1600, 800);
            MinimumSize = new Size(800, 400);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            StyleManager.ApplyBackground(this);

            nicknameBox = MainControls.CreateNicknameBox();
            nicknameBox.Location = new Point(300, 375);
            serverBox = MainControls.CreateServerBox();
            serverBox.Location = new Point(800, 700);
            progressBar = MainControls.CreateProgressBar();
            progressLabel = MainControls.CreateProgressLabel();
            playButton = ButtonControls.CreatePlayButton(async (s, e) => await HandlePlayClick());
            playButton.Location = new Point(1500, 700);


            var settingsButton = ButtonControls.CreateSettingsButton(OpenSettings);

            Controls.AddRange(nicknameBox, serverBox, progressBar, progressLabel, playButton, settingsButton);
        }


        private const string GitHubVerionUrl = "https://raw.githubusercontent.com/Samurai625/samp-launcher/main/version.txt";
        private const string DownloadUrl = "https://github.com/Samurai625/samp-launcher/releases/download/v.1.0.0/GTA.San.Andreas.zip";
        private const string LocalVersionFilePath = "version.txt";

        private async Task<bool> CheckForUpdatesAsync()
        {
            try
            {
                using var httpClient = new HttpClient();
                string remoteVersion = await httpClient.GetStringAsync(GitHubVerionUrl);
                remoteVersion = remoteVersion.Trim();

                string localVersionPath = Path.Combine(gamePath, LocalVersionFilePath);
                string localVersion = File.Exists(localVersionPath) ? File.ReadAllText(localVersionPath).Trim() : "";

               
                    isDownloading = true;
                    playButton.Text = "Завантаження...";

                    string zipPath = Path.Combine(gamePath, "GTA.San.Andreas.zip");
                    var progress = new Progress<int>(p =>
                    {
                        progressBar.Value = p;
                        progressLabel.Text = $"{p}%";
                    });


                    await Downloader.DownloadFileAsync(DownloadUrl, zipPath, progress);
                    await Unpacker.ExtractZipAsync(zipPath, gamePath);
                    File.Delete(zipPath);

                    File.WriteAllText(LocalVersionFilePath, remoteVersion);
                    FileVerifier.SaveFileList(gamePath);

                    isDownloading = false;
                    playButton.Text = "Грати";

                    return true;
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка оновлення: " + ex.Message);
                isDownloading = false;
                playButton.Text = "Грати";
                return false;
            }
        }

        private async Task HandlePlayClick()
        {
            if (isDownloading) return;

            string nickname = nicknameBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(nickname))
            {
                MessageBox.Show("Будь ласка, введіть нікнейм.");
                return;
            }

            if (serverBox.SelectedItem is not ServerItem selectedServer)
            {
                MessageBox.Show("Будь ласка, виберіть сервер.");
                return;
            }

            string sampPath = Path.Combine(gamePath, "samp.exe");

            var fileCheckProgress = new Progress<int>(p =>
           {
               progressBar.Value = p;
               progressLabel.Text = $"{p}%";
           });
            bool fileOk = await FileVerifier.CheckFilesAsync(gamePath, fileCheckProgress);
            if (!fileOk)
            {
                MessageBox.Show("Файли гри пошкоджені або відсутні. Будь ласка, перевірте цілісність файлів.");
                await CheckForUpdatesAsync();
                return;
            }
            GameManager.LaunchGame(gamePath, nickname, selectedServer.Address);
        }

        private void OpenSettings(object? sender, EventArgs e)
        {
            var form = new SettingsForm(gamePath);
            if (form.ShowDialog() == DialogResult.OK)
            {
                gamePath = form.SelectedPath;
                SettingsManager.Save(new SettingsManager.Settings { GamePath = gamePath });
                MessageBox.Show("Шлях до гри збережено.");
            }
        }
    }
}
