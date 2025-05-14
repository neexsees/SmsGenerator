
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
            titleLabel = new RoundLabel();
            countLabel = new Label();
            countInput = new RoundedTextBox();
            generateButton = new RoundedButton();
            buttonHelp = new Button();
            SuspendLayout();
            // 
            // lengthLabel
            // 
            lengthLabel.AutoSize = true;
            lengthLabel.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point);
            lengthLabel.Location = new Point(331, 180);
            lengthLabel.Name = "lengthLabel";
            lengthLabel.Size = new Size(314, 41);
            lengthLabel.TabIndex = 0;
            lengthLabel.Text = "Введите длину кода";
            // 
            // lengthInput
            // 
            lengthInput.BackColor = Color.Transparent;
            lengthInput.Location = new Point(331, 250);
            lengthInput.Name = "lengthInput";
            lengthInput.Padding = new Padding(10);
            lengthInput.Size = new Size(500, 81);
            lengthInput.TabIndex = 1;
            lengthInput.Load += lengthInput_Load;
            // 
            // titleLabel
            // 
            titleLabel.BorderColor = Color.FromArgb(0, 51, 102);
            titleLabel.BorderWidth = 3;
            titleLabel.CornerRadius = 20;
            titleLabel.FillColor = Color.FromArgb(240, 240, 240);
            titleLabel.Font = new Font("Segoe UI", 16F, FontStyle.Bold, GraphicsUnit.Point);
            titleLabel.Location = new Point(0, 0);
            titleLabel.Name = "titleLabel";
            titleLabel.Size = new Size(700, 60);
            titleLabel.TabIndex = 0;
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // countLabel
            // 
            countLabel.AutoSize = true;
            countLabel.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point);
            countLabel.Location = new Point(280, 360);
            countLabel.Name = "countLabel";
            countLabel.Size = new Size(412, 41);
            countLabel.TabIndex = 2;
            countLabel.Text = "Введите количество кодов";
            // 
            // countInput
            // 
            countInput.BackColor = Color.Transparent;
            countInput.Location = new Point(331, 430);
            countInput.Name = "countInput";
            countInput.Padding = new Padding(10);
            countInput.Size = new Size(500, 81);
            countInput.TabIndex = 3;
            // 
            // generateButton
            // 
            generateButton.BackColor = Color.White;
            generateButton.BorderColor = Color.FromArgb(0, 51, 102);
            generateButton.BorderThickness = 3;
            generateButton.CornerRadius = 25;
            generateButton.FlatStyle = FlatStyle.Flat;
            generateButton.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            generateButton.ForeColor = Color.Black;
            generateButton.HoverBackColor = Color.FromArgb(230, 230, 230);
            generateButton.Location = new Point(0, 0);
            generateButton.Name = "generateButton";
            generateButton.Size = new Size(200, 60);
            generateButton.TabIndex = 4;
            generateButton.UseVisualStyleBackColor = false;
            generateButton.Click += GenerateButton_Click;
            // 
            // buttonHelp
            // 
            buttonHelp.Location = new Point(50, 700);
            buttonHelp.Name = "buttonHelp";
            buttonHelp.Size = new Size(200, 50);
            buttonHelp.TabIndex = 5;
            buttonHelp.Text = "Помощь";
            buttonHelp.Click += ButtonHelp_Click;
            // 
            // MainMenu
            // 
            ClientSize = new Size(1200, 800);
            Controls.Add(titleLabel);
            Controls.Add(countLabel);
            Controls.Add(countInput);
            Controls.Add(generateButton);
            Controls.Add(lengthInput);
            Controls.Add(lengthLabel);
            Controls.Add(buttonHelp);
            Name = "MainMenu";
            Text = "Генератор SMS-кодов";
            ResumeLayout(false);
            PerformLayout();

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
                string chmPath = @"C:\Users\Саша\source\repos\SmsGenerator\контекстная справка. Марьина В.ФеденеваА.РИС-24-1.chm";
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

        private void lengthInput_Load(object sender, EventArgs e)
        {

        }
    }
}
