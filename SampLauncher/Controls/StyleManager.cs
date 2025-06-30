using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SAMPLauncher.Forms.Controls
{
    public static class StyleManager
    {
        public static void ApplyBackground(Form form)
        {
            form.BackgroundImageLayout = ImageLayout.Stretch;
            string bgPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resources", "background.png");
            if (File.Exists(bgPath))
                form.BackgroundImage = Image.FromFile(bgPath);
        }
    }
}
