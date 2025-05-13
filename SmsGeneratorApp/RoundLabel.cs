using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SmsGeneratorApp
{
    public class RoundLabel : Label
    {
        public int CornerRadius { get; set; } = 20;
        public Color BorderColor { get; set; } = Color.FromArgb(0, 51, 102); // тёмно-синий
        public int BorderWidth { get; set; } = 3;
        public Color FillColor { get; set; } = Color.FromArgb(240, 240, 240); // светло-серый

        public RoundLabel()
        {
            TextAlign = ContentAlignment.MiddleCenter;
            Font = new Font("Segoe UI", 16, FontStyle.Bold);
            AutoSize = false;
            Height = 60;
            Width = 700;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = ClientRectangle;
            rect.Inflate(-1, -1);

            using var path = GetRoundedRect(rect, CornerRadius);
            using var backBrush = new SolidBrush(FillColor);
            using var borderPen = new Pen(BorderColor, BorderWidth);

            g.FillPath(backBrush, path);
            g.DrawPath(borderPen, path);

            using var textBrush = new SolidBrush(ForeColor);
            var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString(Text, Font, textBrush, rect, sf);
        }

        private GraphicsPath GetRoundedRect(Rectangle rect, int radius)
        {
            int d = radius * 2;
            var path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
