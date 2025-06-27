

namespace SmsGeneratorApp
{
    public partial class CustomParamsForm : Form
    {
        private CheckBox cbA, cbK, cbM;
        private TextBox inputA, inputK, inputM, outputBox;
        private Button calculateButton, debugButton;
        private Label labelA;
        private Label labelM;
        private Label labelK;
        private ListBox resultListBox;
        private RoundedButton clearButton, saveButton, copyButton;
        private HashSet<long> resultHistory = new();
        private long lastA, lastK, lastM;
        private int lastB, lastG, lastD;
        private CheckBox cbLength;
        private TextBox inputLength;

        public CustomParamsForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Генерация со своими параметрами";
            Size = new Size(1200, 800);
            BackColor = Color.White;
            StartPosition = FormStartPosition.CenterParent;

            Font defaultFont = new Font("Segoe UI", 14);

            var titleLabel = new RoundLabel
            {
                BorderColor = Color.FromArgb(0, 51, 102),
                BorderWidth = 4,
                CornerRadius = 20,
                FillColor = Color.FromArgb(240, 240, 240),
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                Location = new Point(150, 20),
                Size = new Size(900, 60),
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "Ручной ввод параметров генерации"
            };

            cbA = new CheckBox
            {
                Location = new Point(100, 135),
            };

            labelA = new Label
            {
                Text = "A",
                Location = new Point(200, 130),  
                Font = new Font("Segoe UI", 12),
                AutoSize = true
            };
            inputA = new TextBox
            {
                Location = new Point(250, 120),
                Width = 300,
                Font = defaultFont,
                Enabled = false
            };
           
            cbA.CheckedChanged += (s, e) => inputA.Enabled = cbA.Checked;
            //m
            cbM = new CheckBox
            {
                Location = new Point(100, 255),
                Font  = new Font("Segoe UI", 10)
            };
            labelM = new Label
            {
                Text = "M",
                Location = new Point(200, 245),  
                Font = new Font("Segoe UI", 12),
                AutoSize = true
            };
            inputM = new TextBox
            {
                Location = new Point(250, 240),
                Width = 300,
                Font = defaultFont,
                Enabled = false
            };
            cbM.CheckedChanged += (s, e) => inputM.Enabled = cbM.Checked;
            //k
            cbK = new CheckBox
            {
                Location = new Point(100, 375),
            };
            labelK = new Label
            {
                Text = "k",
                Location = new Point(200, 365),
                Font = new Font("Segoe UI", 12),
                AutoSize = true
            };
            inputK = new TextBox
            {
                Location = new Point(250, 360),
                Width = 300,
                Font = defaultFont,
                Enabled = false
            };
            cbK.CheckedChanged += (s, e) => inputK.Enabled = cbK.Checked;


            calculateButton = new RoundedButton
            {
                Text = "Вычислить A^K mod M",
                Location = new Point(330, 430),
                Size = new Size(500, 60),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BackColor = Color.White,
                ForeColor = Color.Black,
                BorderColor = Color.FromArgb(0, 51, 102),
                BorderThickness = 3,
                CornerRadius = 25
            };
            calculateButton.Click += CalculateButton_Click;

            debugButton = new RoundedButton
            {
                Text = "Отладка",
                Location = new Point(430, 600),
                Size = new Size(300, 60),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BackColor = Color.White,
                ForeColor = Color.Black,
                BorderColor = Color.FromArgb(0, 51, 102),
                BorderThickness = 3,
                CornerRadius = 25
            };
            debugButton.Click += DebugButton_Click;

            outputBox = new TextBox
            {
                Location = new Point(180, 520),
                Width = 800,
                Font = defaultFont,
                ReadOnly = true
            };


            resultListBox = new ListBox
            {
                Location = new Point(600, 100),
                Size = new Size(500, 200),
                Font = new Font("Segoe UI", 12),
            };

            clearButton = new RoundedButton
            {
                Text = "Очистить",
                Location = new Point(580, 300),
                Size = new Size(150, 40),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BorderColor = Color.FromArgb(0, 51, 102),
                BorderThickness = 2,
                CornerRadius = 15
            };
            clearButton.Click += (s, e) =>
            {
                resultHistory.Clear();
                resultListBox.Items.Clear();
            };

            copyButton = new RoundedButton
            {
                Text = "Cкопировать",
                Location = new Point(950, 300),
                Size = new Size(180, 40),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BorderColor = Color.FromArgb(0, 51, 102),
                BorderThickness = 2,
                CornerRadius = 15
            };
            copyButton.Click += (s, e) =>
            {
                Clipboard.SetText(string.Join("\n", resultHistory));
                MessageBox.Show("Скопировано!", "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            saveButton = new RoundedButton
            {
                Text = "Сохранить(.txt)",
                Location = new Point(735, 300),
                Size = new Size(210, 40),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BorderColor = Color.FromArgb(0, 51, 102),
                BorderThickness = 2,
                CornerRadius = 15
            };
            saveButton.Click += (s, e) =>
            {
                using SaveFileDialog sfd = new()
                {
                    Filter = "Текстовые файлы (*.txt)|*.txt",
                    FileName = "codes.txt"
                };
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllLines(sfd.FileName, resultHistory.Select(c => c.ToString()).ToList());
                    MessageBox.Show("Сохранено!", "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };


            Controls.AddRange(new Control[]
            {
                titleLabel,
                cbA, labelA, inputA,
                cbK, labelK, inputK,
                cbM, labelM, inputM,
                calculateButton,
                debugButton,
                outputBox,
                resultListBox,
                clearButton,
                saveButton,
                copyButton
            });

        }

        private void CalculateButton_Click(object sender, EventArgs e)
        {
            try
            {
                // M
                lastM = cbM.Checked
                    ? long.Parse(inputM.Text)
                    : CodeGenerator.FindValidModulus(6, 1, out _, out _, out _, out _);

                // A (подбирается взаимнопросто к M)
                if (cbA.Checked)
                {
                    lastA = long.Parse(inputA.Text);
                    if (!CodeGenerator.MutualSimplicity((int)lastA, (int)lastM))
                        throw new Exception("A и M не взаимно просты.");
                }
                else
                {
                    do
                    {
                        lastA = CodeGenerator.GeneratePrime(6);
                    } while (!CodeGenerator.MutualSimplicity((int)lastA, (int)lastM));
                }

                // K (всегда случайное, если не задано)
                if (cbK.Checked)
                {
                    lastK = long.Parse(inputK.Text);
                    lastB = lastG = lastD = -1; // Явно указать, что не авто
                }
                else
                {
                    // Получаем b и g из генератора
                    (lastB, lastG) = CodeGenerator.GeneratePrimesBG(6);

                    // Генерируем d и считаем k = b^d mod g
                    lastD = new Random().Next(1, lastG); // d ∈ [1; g-1]
                    lastK = CodeGenerator.PowMod(lastB, lastD, lastG);
                }


                // Вычисление
                long result = CodeGenerator.PowMod(lastA, lastK, lastM);
                if (result < 0) result = (result + lastM) % lastM;

                outputBox.Text = $"{result}";
                if (resultHistory.Add(result)) // добавится только если уникальный
                {
                    resultListBox.Items.Add(result);
                }
                else
                {
                    MessageBox.Show("Такой код уже был сгенерирован. Повтор исключён.", "Повтор", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void DebugButton_Click(object sender, EventArgs e)
        {
            string message = $"Параметры:\n\nA = {lastA}\nK = {lastK}\nM = {lastM}\n\nA^K mod M = {CodeGenerator.PowMod(lastA, lastK, lastM)}";
            if (lastD > 0)
            {
                message += $"\n\nФормула для K: {lastB}^{lastD} mod {lastG} = {lastK}";
            }
            MessageBox.Show(message, "Отладочная информация", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }
    }
}
