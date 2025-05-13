using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SmsGeneratorApp
{
    public class RoundedButton : Button
    {
        public int CornerRadius { get; set; } = 25;
        public Color BorderColor { get; set; } = Color.FromArgb(0, 51, 102);
        public int BorderThickness { get; set; } = 3;
        public Color HoverBackColor { get; set; } = Color.FromArgb(230, 230, 230);

        private bool isHovered = false;

        public RoundedButton()
        {
            FlatStyle = FlatStyle.Flat;
            BackColor = Color.White;
            ForeColor = Color.Black;
            Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            FlatAppearance.BorderSize = 0;
            Cursor = Cursors.Hand;
            Size = new Size(200, 60);

            MouseEnter += (s, e) => { isHovered = true; Invalidate(); };
            MouseLeave += (s, e) => { isHovered = false; Invalidate(); };
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            var path = GetRoundPath(rect, CornerRadius);

            var shadowRect = new Rectangle(3, 3, Width - 1, Height - 1);
            var shadowPath = GetRoundPath(shadowRect, CornerRadius);
            using (var shadowBrush = new SolidBrush(Color.FromArgb(40, Color.Black)))
                g.FillPath(shadowBrush, shadowPath);


            using (var brush = new SolidBrush(isHovered ? HoverBackColor : BackColor))
                g.FillPath(brush, path);

            using (var pen = new Pen(BorderColor, BorderThickness))
                g.DrawPath(pen, path);

            TextRenderer.DrawText(g, Text, Font, rect, ForeColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        private GraphicsPath GetRoundPath(Rectangle r, int radius)
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
