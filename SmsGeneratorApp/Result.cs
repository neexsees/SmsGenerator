

namespace SmsGeneratorApp
{
    public partial class Result: Form
    {
        private Label successLabel;
        private Label codesLabel;
        private TrackBar fontSizeSlider;
        private Panel codesPanel;

        private List<long> codes;

        public Result(List<long> codes)
        {
            this.codes = codes;

            Text = "Итог";
            Size = new Size(1200, 800);
            BackColor = Color.White;

            InitializeComponents();
        }
        private void InitializeComponents()
        {
            // Заголовок
            var title = new RoundLabel
            {
                Text = "Генератор псевдослучайных SMS-кодов",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
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
                BackColor = this.BackColor,
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
                Location = new Point(350, 560),
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
                        // Сохраняем коды по одному в строке
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

        }
    }

}

