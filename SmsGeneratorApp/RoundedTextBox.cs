using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SmsGeneratorApp
{
    public class RoundedTextBox : UserControl
    {
        private readonly TextBox innerTextBox = new TextBox();

        public RoundedTextBox()
        {
            this.Padding = new Padding(10);
            innerTextBox.BorderStyle = BorderStyle.None;
            innerTextBox.Font = new Font("Segoe UI", 16);
            innerTextBox.BackColor = Color.FromArgb(240, 240, 240);
            innerTextBox.Dock = DockStyle.Fill;

            this.Controls.Add(innerTextBox);
            this.Size = new Size(500, 50);
            this.BackColor = Color.Transparent;
        }

        public override string Text
        {
            get => innerTextBox.Text;
            set => innerTextBox.Text = value;
        }

        public TextBox InnerBox => innerTextBox;

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
            using var path = GetRoundRect(rect, 20);
            using var brush = new SolidBrush(Color.FromArgb(240, 240, 240));
            using var pen = new Pen(Color.FromArgb(0, 51, 102), 4);

            g.FillPath(brush, path);
            g.DrawPath(pen, path);
        }

        private GraphicsPath GetRoundRect(Rectangle r, int radius)
        {
            int d = radius * 2;
            GraphicsPath path = new GraphicsPath();
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
