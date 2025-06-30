using System.Drawing;
using System.Windows.Forms;
using SAMPLauncher.Utils;
using System.Drawing.Drawing2D;

namespace SAMPLauncher.Forms.Controls
{
    public static class MainControls
    {
        public static TextBox CreateNicknameBox()
        {
            TextBox nicknameBox = new TextBox
            {
                Width = 200,
                Height = 30,
                Font = new Font("Segoi UI", 12),
                ForeColor = Color.Black,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            MakeRoundedTextBox(nicknameBox, 30);
            return nicknameBox;
        }

        public static ComboBox CreateServerBox()
        {
            var box = new ComboBox
            {
                Width = 100,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            box.Items.Add(new ServerItem("Redgate", "51.81.48.135:7777"));
            box.SelectedIndex = 0;
            return box;
        }

        public static ProgressBar CreateProgressBar()
        {
            return new ProgressBar
            {
                Width = 400
            };
        }

        public static Label CreateProgressLabel()
        {
            return new Label
            {
                Width = 40,
                Text = "0%"
            };
        }
        private static void MakeRoundedTextBox(TextBox box, int radius)
        {
            var path = new GraphicsPath();
            path.StartFigure();
            path.AddArc(new Rectangle(0, 0, radius, radius), 180, 95);
            path.AddArc(new Rectangle(box.Width - radius, 1, radius, radius), 275, 100);
            path.AddArc(new Rectangle(box.Width - radius, box.Height - radius, radius, radius), 5, 90);
            path.AddArc(new Rectangle(0, box.Height - radius, radius, radius), 95, 90);
            path.CloseFigure();

            box.Region = new Region(path);

            box.Resize += (s,e) => MakeRoundedTextBox(box, radius);
        }
    }
}
