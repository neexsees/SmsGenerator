
using System.Diagnostics;

namespace SmsGeneratorApp
{
    public partial class MainMenu : Form
    {
        private RoundedTextBox lengthInput;
        private RoundedTextBox countInput;
        private Label countLabel;
        private RoundedButton generateButton;
        private Button buttonHelp;
        public MainMenu()
        {
            InitializeComponent();

        }

        private void InitializeComponent()
        {
            this.BackColor = Color.White;
            lengthLabel = new Label();
            lengthInput = new RoundedTextBox();
            SuspendLayout();
            // Заголовок
            titleLabel = new RoundLabel
            {
                BorderColor = Color.FromArgb(0, 51, 102),
                BorderWidth = 8,
                CornerRadius = 20,
                FillColor = Color.FromArgb(240, 240, 240),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Location = new Point(180, 30),
                Size = new Size(900, 60),
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "Генератор одноразовых псевдослучайных SMS-кодов"
            };
            Controls.Add(titleLabel);
            //Текст перед вводом длины
            // 
            // lengthLabel
            // 
            lengthLabel.AutoSize = true;
            lengthLabel.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point);
            lengthLabel.Location = new Point(331, 180);
            lengthLabel.Name = "lengthLabel";
            lengthLabel.Size = new Size(501, 65);
            lengthLabel.TabIndex = 0;
            lengthLabel.Text = "Введите длину кода";
            //Ввод длины
            // 
            // lengthInput
            // 
            lengthInput.BackColor = Color.Transparent;
            lengthInput.Location = new Point(331, 250);
            lengthInput.Name = "lengthInput";
            lengthInput.Padding = new Padding(10);
            lengthInput.Size = new Size(500, 81);
            lengthInput.TabIndex = 1;
            //Текст для ввод количества
            countLabel = new Label();
            countLabel.AutoSize = true;
            countLabel.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point);
            countLabel.Location = new Point(280, 360); 
            countLabel.Name = "countLabel";
            countLabel.Size = new Size(501, 65);
            countLabel.TabIndex = 2;
            countLabel.Text = "Введите количество кодов";
            //Ввод количества
            countInput = new RoundedTextBox();
            countInput.BackColor = Color.Transparent;
            countInput.Location = new Point(331, 430);
            countInput.Name = "countInput";
            countInput.Padding = new Padding(10);
            countInput.Size = new Size(500, 81);
            countInput.TabIndex = 3;


            Controls.Add(countLabel);
            Controls.Add(countInput);
            //Кнопка генерации
            generateButton = new RoundedButton
            {
                Text = "Сгенерировать",
                Location = new Point(440, 550),
                Size = new Size(300, 60),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BackColor = Color.White,      
                ForeColor = Color.Black,         
                BorderColor = Color.FromArgb(0, 51, 102),
                BorderThickness = 3,
                CornerRadius = 25
            };
            generateButton.Click += GenerateButton_Click;
            Controls.Add(generateButton);

            var generateCustomButton = new RoundedButton
            {
                Text = "Свои параметры",
                Location = new Point(440, 650),
                Size = new Size(300, 60),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BackColor = Color.White,
                ForeColor = Color.Black,
                BorderColor = Color.FromArgb(0, 51, 102),
                BorderThickness = 3,
                CornerRadius = 25
            };
            generateCustomButton.Click += GenerateCustomButton_Click;
            Controls.Add(generateCustomButton);
            // 
            // MainMenu
            // 
            ClientSize = new Size(1200, 800);
            Controls.Add(lengthInput);
            Controls.Add(lengthLabel);
            Name = "MainMenu";
            Text = "Генератор одноразовых псевдослучайных SMS-кодов";
            ResumeLayout(false);
            PerformLayout();

            buttonHelp = new Button();
            buttonHelp.Text = "Помощь";
            buttonHelp.Size = new Size(200, 50);
            buttonHelp.Location = new Point(50, 700); 
            buttonHelp.Click += ButtonHelp_Click;
            Controls.Add(buttonHelp);

        }
        private bool TryGetLength(out long codeLength)
        {
            codeLength = 0;
            string input = lengthInput.Text.Trim();

            if (!long.TryParse(input, out long value) || value <= 0 || value < 6 || value > 9)
            {
                MessageBox.Show("Введите корректную длину кода (целое число от 6 до 9).",
                    "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            codeLength = value;
            return true;
        }

        private bool TryGetCount(out long numberOfCodes)
        {
            numberOfCodes = 0;
            string input = countInput.Text.Trim();

            if (!long.TryParse(input, out long value) || value < 1 || value > 100)
            {
                MessageBox.Show("Введите количество кодов от 1 до 100.",
                    "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            numberOfCodes = value;
            return true;
        }
        private async void GenerateButton_Click(object sender, EventArgs e)
        {
            if (TryGetLength(out long length) && TryGetCount(out long numberOfCodes))
            {
                try
                {
                    long m = 0, a = 0, p = 0, q = 0;
                    List<long> usedK = new List<long>();
                    int b = 0, g = 0;
                    var result = await Task.Run(() =>
                    {
                        return CodeGenerator.GenerateCodes((int)length, (int)numberOfCodes, out m, out a, out p, out q, out usedK, out b, out g);
                    });
                    Debug.WriteLine($"Сгенерировано кодов: {result.Count}");

                    var resultForm = new Result(result, a, m, p, q, usedK, b, g);
                    resultForm.Show();
                    this.Hide();

                    resultForm.FormClosed += (s, args) =>
                    {
                        this.Show();
                    };
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка генерации: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
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
        private void GenerateCustomButton_Click(object sender, EventArgs e)
        {
            var customParamsForm = new CustomParamsForm();
            customParamsForm.Show();
            this.Hide();

            customParamsForm.FormClosed += (s, args) =>
            {
                this.Show();
            };
        }
    }
}
