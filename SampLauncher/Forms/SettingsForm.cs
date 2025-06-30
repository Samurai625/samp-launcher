using System;
using System.Windows.Forms;

public class SettingsForm : Form
{
    private TextBox pathBox;
    private Button browseButton;
    private Button okButton;
    private FolderBrowserDialog folderBrowser;

    public string SelectedPath => pathBox.Text;

    public SettingsForm(string currentPath)
    {
        Text = "Налаштування";
        Width = 400;
        Height = 150;

        pathBox = new TextBox { Left = 10, Top = 10, Width = 250, Text = currentPath };
        browseButton = new Button { Text = "Огляд", Left = 270, Top = 8, Width = 100 };
        okButton = new Button { Text = "OK", Left = 150, Top = 50, Width = 100 };

        folderBrowser = new FolderBrowserDialog();

        browseButton.Click += (sender, e) =>
        {
            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                pathBox.Text = folderBrowser.SelectedPath;
            }
        };

        okButton.Click += (sender, e) => DialogResult = DialogResult.OK;

        Controls.Add(pathBox);
        Controls.Add(browseButton);
        Controls.Add(okButton);
    }
}
