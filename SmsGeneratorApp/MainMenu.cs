
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
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                Location = new Point(180, 30),
                Size = new Size(900, 60),
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "Генератор псевдослучайных SMS-кодов"
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
            // 
            // MainMenu
            // 
            ClientSize = new Size(1200, 800);
            Controls.Add(lengthInput);
            Controls.Add(lengthLabel);
            Name = "MainMenu";
            Text = "Генератор SMS-кодов";
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

            if (!long.TryParse(input, out long value) || value <= 0)
            {
                MessageBox.Show("Введите корректную длину кода (целое положительное число).",
                    "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            codeLength = value;
            return true;
        }
        private bool TryGetCount(out long codeCount)
        {
            codeCount = 0;
            string input = countInput.Text.Trim();

            if (!long.TryParse(input, out long value) || value <= 0)
            {
                MessageBox.Show("Введите корректное количество кодов (целое положительное число).",
                    "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            codeCount = value;
            return true;
        }

        private void GenerateButton_Click(object sender, EventArgs e)
        {
            if (TryGetLength(out long length) && TryGetCount(out long count))
            {
                if (!CodeGenerator.FindSuitableParameters(length, count, out long a, out long n))
                {
                    MessageBox.Show("Не удалось найти параметры a и n.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                try
                {
                    var codes = CodeGenerator.MakeCodes(a, n, length, count);
                    var resultForm = new Result(codes);
                    resultForm.Show(); 
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка генерации: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void ButtonHelp_Click(object sender, EventArgs e)
        {
            string pdfPath = @"C:\Users\Redmi\Desktop\hse\1 КУРС\КУРСАЧ\SmsGenerator\UserGuide.pdf";
            if (File.Exists(pdfPath))
            {
                System.Diagnostics.Process.Start(new ProcessStartInfo()
                {
                    FileName = pdfPath,
                    UseShellExecute = true
                });
            }
            else
            {
                MessageBox.Show("Файл PDF не найден.");
            }
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F1)
            {
                string pdfPath = @"C:\Users\Redmi\Desktop\hse\docs\UserGuide.pdf";
                if (File.Exists(pdfPath))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = pdfPath,
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
    }
}
