

using System.Diagnostics;
using Microsoft.VisualBasic.ApplicationServices;

namespace SmsGeneratorApp
{
    public partial class Result: Form
    {
        private Label successLabel;
        private long a;
        private long m;
        private long p;
        private long q;
        private int b;
        private int g;
        private List<long> usedK;
        private List<long> codes;
        private Button buttonHelp;


        public Result(List<long> codes, long a, long m, long p, long q, List<long> usedK, int b, int g)
        {
            this.codes = codes;
            this.a = a;
            this.m = m;
            this.p = p;
            this.q = q;
            this.usedK = usedK;
            this.b = b;
            this.g = g;

            Text = "Сгенерированные коды";
            Size = new Size(1200, 800);
            BackColor = Color.White;

            InitializeComponents();
        }

        private void InitializeComponents()
        {
            // Заголовок
            var title = new RoundLabel
            {
                Text = "Генератор одноразовых псевдослучайных SMS-кодов",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Size = new Size(900, 60),
                Location = new Point(150, 30),
                BorderWidth = 4,
                BorderColor = Color.FromArgb(0, 51, 102),
                CornerRadius = 20,
                FillColor = Color.FromArgb(240, 240, 240),
                TextAlign = ContentAlignment.MiddleCenter
            };
            Controls.Add(title);

            // Подпись об успешной генерации
            successLabel = new Label
            {
                Text = "Коды успешно сгенерированы!",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(180, 120)
            };
            Controls.Add(successLabel);

            //прокрутка
            var codesBox = new TextBox
            {
                Text = string.Join(" ", codes),
                Location = new Point(100, 200),
                Size = new Size(1000, 100),             
                Font = new Font("Segoe UI", 20),
                ReadOnly = true,
                BorderStyle = BorderStyle.None,
                Multiline = true,                     
                WordWrap = false,                        
                ScrollBars = ScrollBars.Horizontal,      
                BackColor = Color.White,
                TabStop = false
            };
            Controls.Add(codesBox);

            var copyButton = new RoundedButton
            {
                Text = "Скопировать",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Size = new Size(500, 80),
                Location = new Point(350, 380), 
                BorderColor = Color.FromArgb(0, 51, 102),
                BorderThickness = 4,
                CornerRadius = 30
            };
            copyButton.Click += (s, e) =>
            {
                Clipboard.SetText(string.Join(" ", codes));
                MessageBox.Show("Коды скопированы в буфер обмена!", "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };
            Controls.Add(copyButton);

            var exportButton = new RoundedButton
            {
                Text = "Выгрузить в файл .txt",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Size = new Size(500, 80),
                Location = new Point(350, 490),
                BorderColor = Color.FromArgb(0, 51, 102),
                BorderThickness = 4,
                CornerRadius = 30
            };

            exportButton.Click += (s, e) =>
            {
                using var sfd = new SaveFileDialog
                {
                    Filter = "Текстовые файлы (*.txt)|*.txt",
                    Title = "Сохранить коды",
                    FileName = "sms_codes.txt"
                };

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        File.WriteAllLines(sfd.FileName, codes.ConvertAll(c => c.ToString()));

                        MessageBox.Show("Файл успешно сохранён!", "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при сохранении:\n{ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            };
            Controls.Add(exportButton);

            var paramsButton = new RoundedButton
            {
                Text = "Отладка",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Size = new Size(500, 80),
                Location = new Point(350, 600),
                BorderColor = Color.FromArgb(0, 51, 102),
                BorderThickness = 4,
                CornerRadius = 30
            };

            paramsButton.Click += (s, e) =>
            {
                var paramForm = new ParametersForm(a, m, p, q, usedK, b, g);
                paramForm.ShowDialog();
            };
            Controls.Add(paramsButton);

            buttonHelp = new Button();
            buttonHelp.Text = "Помощь";
            buttonHelp.Size = new Size(200, 50);
            buttonHelp.Location = new Point(50, 640);
            buttonHelp.Click += ButtonHelp_Click;
            buttonHelp.BackColor = Color.White;
            Controls.Add(buttonHelp);

        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F1)
            {
                string chmPath = @"C:\Users\Redmi\Desktop\hse\1 КУРС\КУРСАЧ\SmsGenerator\UserGuide.chm";
                if (File.Exists(chmPath))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = chmPath,
                        UseShellExecute = true
                    });
                }
                else
                {
                    MessageBox.Show("Файл справки не найден.");
                }
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
        private void ButtonHelp_Click(object sender, EventArgs e)
        {
            string chmPath = @"C:\Users\Redmi\Desktop\hse\1 КУРС\КУРСАЧ\SmsGenerator\UserGuide.chm";
            if (File.Exists(chmPath))
            {
                System.Diagnostics.Process.Start(new ProcessStartInfo()
                {
                    FileName = chmPath,
                    UseShellExecute = true
                });
            }
            else
            {
                MessageBox.Show("Файл не найден.");
            }
        }

    }

}

