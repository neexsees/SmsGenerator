using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SmsGeneratorApp
{
    public partial class ParametersForm : Form
    {
        public ParametersForm(long a, long m, List<long> kList)
        {
            Text = "Параметры генерации";
            Size = new Size(800, 650);
            BackColor = Color.White;
            StartPosition = FormStartPosition.CenterParent;

            InitializeComponents(a, m, kList);
        }

        private void InitializeComponents(long a, long m, List<long> kList)
        {
            var title = new RoundLabel
            {
                Text = "Использованные параметры генерации",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Size = new Size(700, 50),
                Location = new Point(50, 20),
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

            var kHeader = new Label
            {
                Text = "Использованные значения k:",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                Location = new Point(60, 190),
                AutoSize = true
            };
            Controls.Add(kHeader);

            var kBox = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                ReadOnly = true,
                Location = new Point(60, 230),
                Size = new Size(660, 280),
                Font = new Font("Segoe UI", 12),
                Text = string.Join(", ", kList),
                BackColor = Color.White
            };
            Controls.Add(kBox);

            var closeButton = new RoundedButton
            {
                Text = "Закрыть",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Size = new Size(200, 50),
                Location = new Point(300, 540),
                BorderColor = Color.FromArgb(0, 51, 102),
                BorderThickness = 3,
                CornerRadius = 20,
                BackColor = Color.White,
                ForeColor = Color.Black
            };
            closeButton.Click += (s, e) => this.Close();
            Controls.Add(closeButton);
        }
    }
}
