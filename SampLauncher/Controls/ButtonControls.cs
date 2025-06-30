using System;
using System.Drawing;
using System.Windows.Forms;

namespace SAMPLauncher.Forms.Controls
{
    public static class ButtonControls
    {
        public static Button CreatePlayButton(EventHandler clickHandler)
        {
            var button = new Button
            {
                Text = "Грати",
                Width = 100
            };
            button.Click += clickHandler;
            return button;
        }

        public static Button CreateSettingsButton(EventHandler clickHandler)
        {
            var button = new Button
            {
                Text = "⚙️",
                Size = new Size(30, 30),
                Location = new Point(760, 10)
            };
            button.Click += clickHandler;
            return button;
        }
    }
}
