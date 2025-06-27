
namespace SmsGeneratorApp
{
    public partial class ParametersForm : Form
    {
        public ParametersForm(long a, long m, long p, long q, List<long> kList, int b, int g)
        {
            Text = "Параметры генерации";
            Size = new Size(1200, 800);
            BackColor = Color.White;
            StartPosition = FormStartPosition.CenterParent;

            InitializeComponents(a, m, p, q, kList, b, g);
        }

        private void InitializeComponents(long a, long m, long p, long q, List<long> kList, int b, int g)
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

            Controls.Add(new Label
            {
                Text = $"a = {a}",
                Font = new Font("Segoe UI", 14),
                Location = new Point(60, 100),
                AutoSize = true
            });

            Controls.Add(new Label
            {
                Text = $"m = p × q = {p} × {q} = {m}",
                Font = new Font("Segoe UI", 14),
                Location = new Point(60, 140),
                AutoSize = true
            });

            Controls.Add(new Label
            {
                Text = $"Параметры генерации степени k: b = {b}, g = {g}",
                Font = new Font("Segoe UI", 14),
                Location = new Point(60, 180),
                AutoSize = true
            });

            Controls.Add(new Label
            {
                Text = "Использованные значения k:",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(340, 230),
                AutoSize = true
            });

            Controls.Add(new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                ReadOnly = true,
                Location = new Point(220, 310),
                Size = new Size(800, 350),
                Font = new Font("Segoe UI", 12),
                Text = string.Join(", ", kList),
                BackColor = Color.White
            });
        }
    }
}
