using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SmsGeneratorApp
{
    public partial class ParametersForm : Form
    {
        public ParametersForm(long a, long m, long p, long q, List<long> kList)
        {
            Text = "Параметры генерации";
            Size = new Size(1200, 800);
            BackColor = Color.White;
            StartPosition = FormStartPosition.CenterParent;

            InitializeComponents(a, m, p, q, kList);
        }

        private void InitializeComponents(long a, long m, long p, long q, List<long> kList)
        {
            var title = new RoundLabel
            {
                Text = "Использованные параметры генерации",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Size = new Size(900, 60),
                Location = new Point(180, 30),
                BorderWidth = 4,
                BorderColor = Color.FromArgb(0, 51, 102),
                CornerRadius = 15,
                FillColor = Color.FromArgb(240, 240, 240),
                TextAlign = ContentAlignment.MiddleCenter
            };
            Controls.Add(title);

            var aLabel = new Label
            {
                Text = $"a = {a}",
                Font = new Font("Segoe UI", 14, FontStyle.Regular),
                Location = new Point(60, 100),
                AutoSize = true
            };
            Controls.Add(aLabel);

            var mLabel = new Label
            {
                Text = $"m = {m}",
                Font = new Font("Segoe UI", 14, FontStyle.Regular),
                Location = new Point(60, 140),
                AutoSize = true
            };
            Controls.Add(mLabel);

            var mFullLabel = new Label
            {
                Text = $"m = p × q = {p} × {q} = {m}",
                Font = new Font("Segoe UI", 14, FontStyle.Italic),
                Location = new Point(60, 180),
                AutoSize = true
            };
            Controls.Add(mFullLabel);

            var kHeader = new Label
            {
                Text = "Использованные значения k:",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                Location = new Point(340, 260),
                AutoSize = true
            };
            Controls.Add(kHeader);

            var kBox = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                ReadOnly = true,
                Location = new Point(220, 310),
                Size = new Size(800, 350),
                Font = new Font("Segoe UI", 12),
                Text = string.Join(", ", kList),
                BackColor = Color.White
            };
            Controls.Add(kBox);
        }
    }
}
