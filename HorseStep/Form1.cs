    namespace HorseStep
    {
    public partial class Form1 : Form
    {
        private readonly int[] dx = { 2, 2, 1, 1, -1, -1, -2, -2 }; // Возможные смещения по x
        private readonly int[] dy = { 1, -1, 2, -2, 2, -2, 1, -1 }; // Возможные смещения по y
        private TextBox[,] textBoxes; // Массив для хранения ссылок на текстовые поля
        private bool[,] visited; // Массив для хранения информации, посещалась ли клетка
        private int currentStep = 0;  // Номер текущего хода
        private int startX, startY;    // Начальные координаты коня
        private int delay = 200;  // Задержка между ходами (по умолчанию)



        public Form1()
        {
            InitializeComponent();

            // Устанавливаем диапазон значений ползунка
            delayTrackBar.Minimum = 0;
            delayTrackBar.Maximum = 1000;
            delayTrackBar.Value = delay; // Начальная задержка
            delayTrackBar.TickFrequency = 100; // Шаг изменения

            // Добавляем обработчик для изменения значения ползунка
            delayTrackBar.Scroll += new EventHandler(delayTrackBar_Scroll);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string nText = sizeN.Text;
            string mText = sizeM.Text;

            if (int.TryParse(nText, out int n) && int.TryParse(mText, out int m))
            {
                // Очищаем предыдущие элементы
                panel.Controls.Clear();

                // Создаем TableLayoutPanel
                TableLayoutPanel tableLayoutPanel = new TableLayoutPanel();
                tableLayoutPanel.RowCount = m;
                tableLayoutPanel.ColumnCount = n;
                tableLayoutPanel.Dock = DockStyle.Fill;

                // Устанавливаем одинаковый размер для всех строк и столбцов
                for (int i = 0; i < m; i++)
                {
                    tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100f / m));
                }
                for (int j = 0; j < n; j++)
                {
                    tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / n));
                }

                // Инициализируем массив текстовых полей
                textBoxes = new TextBox[m, n];
                visited = new bool[m, n];

                // Заполняем TableLayoutPanel текстовыми полями и сохраняем ссылки на них
                for (int i = 0; i < m; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        TextBox textBox = new TextBox();
                        textBox.Dock = DockStyle.Fill;
                        textBox.Multiline = true;
                        textBox.ReadOnly = true;

                        // Раскраска шахматной доски
                        if ((i + j) % 2 == 0)
                        {
                            textBox.BackColor = Color.White;  // Белая клетка
                        }
                        else
                        {
                            textBox.BackColor = Color.Gray;   // Черная клетка
                        }

                        textBoxes[i, j] = textBox;
                        tableLayoutPanel.Controls.Add(textBox, j, i);
                    }
                }


                // Добавляем TableLayoutPanel в panel2
                panel.Controls.Add(tableLayoutPanel);

                // Начальные координаты коня (можно задать через пользовательский ввод)
                startX = int.TryParse(StartPointX.Text, out int parsedX) ? parsedX : 0;
                startY = int.TryParse(StartPointY.Text, out int parsedY) ? parsedY : 0;

                // Сбрасываем номер хода
                currentStep = 0;

                // Записываем стартовый ход
                MakeKnightMove(startX, startY);
            }
            else
            {
                MessageBox.Show("Пожалуйста, введите корректные значения для N и M.");
            }
        }

        private async Task<bool> MakeKnightMove(int x, int y)
        {
            // Проверяем выход за пределы доски или если клетка уже посещена
            if (x < 0 || y < 0 || x >= textBoxes.GetLength(0) || y >= textBoxes.GetLength(1) || visited[x, y])
            {
                return false;
            }

            // Записываем номер хода в текстовое поле через Invoke
            if (textBoxes[x, y].InvokeRequired)
            {
                textBoxes[x, y].Invoke(new Action(() => textBoxes[x, y].Text = currentStep.ToString()));
            }
            else
            {
                textBoxes[x, y].Text = currentStep.ToString();
            }

            // Помечаем клетку как посещенную
            visited[x, y] = true;

            // Обновляем номер хода
            currentStep++;

            // Добавляем задержку для обновления интерфейса, используя текущее значение ползунка
            await Task.Delay(delay);

            // Если все клетки посещены, возвращаем true
            if (currentStep == textBoxes.Length)
            {
                return true;
            }

            // Пробуем все 8 возможных ходов
            for (int i = 0; i < 8; i++)
            {
                int newX = x + dx[i];
                int newY = y + dy[i];

                if (await MakeKnightMove(newX, newY))
                {
                    return true;
                }
            }

            // Если ни один ход не сработал, откатываем ход и помечаем клетку как непосещенную
            visited[x, y] = false;

            // Очищаем текстовое поле при возврате
            if (textBoxes[x, y].InvokeRequired)
            {
                textBoxes[x, y].Invoke(new Action(() => textBoxes[x, y].Text = ""));
            }
            else
            {
                textBoxes[x, y].Text = "";
            }

            // Уменьшаем номер хода (но не сбрасываем его)
            currentStep--;

            return false;
        }


private (int, int) FindNextMove(int x, int y)
        {
            List<(int, int)> possibleMoves = new List<(int, int)>();

            // Поиск всех возможных ходов
            for (int i = 0; i < 8; i++)
            {
                int newX = x + dx[i];
                int newY = y + dy[i];

                if (IsValidMove(newX, newY))
                {
                    possibleMoves.Add((newX, newY));
                }
            }

            // Выбираем ход, который ведет на клетку с минимальным количеством возможных следующих ходов
            var nextMove = possibleMoves
                .OrderBy(move => CountAvailableMoves(move.Item1, move.Item2))
                .FirstOrDefault();

            return nextMove != default ? nextMove : (-1, -1); // Если ходов нет, возвращаем (-1, -1)
        }

        private int CountAvailableMoves(int x, int y)
        {
            int[] dx = { 2, 2, -2, -2, 1, 1, -1, -1 };
            int[] dy = { 1, -1, 1, -1, 2, -2, 2, -2 };
            int count = 0;

            for (int i = 0; i < 8; i++)
            {
                int newX = x + dx[i];
                int newY = y + dy[i];

                if (IsValidMove(newX, newY))
                {
                    count++;
                }
            }

            return count;
        }

        private bool IsValidMove(int x, int y)
        {
            return x >= 0 && y >= 0 && x < textBoxes.GetLength(0) && y < textBoxes.GetLength(1) && textBoxes[x, y].Text == "";
        }

        private void delayTrackBar_Scroll(object sender, EventArgs e)
        {
            delay = delayTrackBar.Value; // Устанавливаем задержку по значению ползунка
        }
    }
}
