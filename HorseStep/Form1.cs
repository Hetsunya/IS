using System.Diagnostics;

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
        private bool timerRunning = false;
        private double elapsedTime = 0; // Счетчик времени в секундах

        public Form1()
        {
            InitializeComponent();

            // Устанавливаем диапазон значений ползунка
            delayTrackBar.Minimum = 0;
            delayTrackBar.Maximum = 1000;
            delayTrackBar.Value = delay; // Начальная задержка
            delayTrackBar.TickFrequency = 10; // Шаг изменения

            // Добавляем обработчик для изменения значения ползунка
            delayTrackBar.Scroll += new EventHandler(delayTrackBar_Scroll);

            this.Text = "Задача о ходе коня";
        }


        private void delayTrackBar_Scroll(object sender, EventArgs e)
        {
            delay = delayTrackBar.Value;
        }

        private void OpenCsvFile(string filePath)
        {
            try
            {
                // Создаём новый процесс для открытия файла
                Process.Start(new ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось открыть файл: {ex.Message}");
            }
        }


        private void WriteStartInfoToCSV(string filePath, int startX, int startY, int boardWidth, int boardHeight)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("Запуск; Начальные координаты X; Начальные координаты Y; Ширина; Высота");
                writer.WriteLine($"Запуск; {startX}; {startY}; {boardWidth}; {boardHeight}");
            }
        }

        private void WriteMovesToCSV(string filePath, List<Point> path)
        {
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine("Шаг; X; Y");
                for (int i = 0; i < path.Count; i++)
                {
                    writer.WriteLine($"{i}; {path[i].X + 1}; {path[i].Y + 1}");
                }
            }
        }


        // Метод для обновления времени
        private async void StartTimer()
        {
            timerRunning = true;
            elapsedTime = 0;

            while (timerRunning)
            {
                await Task.Delay(10); // Задержка на 100 миллисекунд для точности до десятых долей
                elapsedTime += 0.01; // Увеличиваем время на 0.1 секунды (100 мс)

                // Обновляем интерфейс через UI-поток
                if (InvokeRequired)
                {
                    Invoke(new Action(() =>
                    {
                        // Форматируем время с двумя знаками после запятой
                        timeLabel.Text = $"Прошло времени: {elapsedTime:F2} сек";
                    }));
                }
                else
                {
                    timeLabel.Text = $"Прошло времени: {elapsedTime:F2} сек";
                }
            }
        }

        private void StopTimer()
        {
            timerRunning = false;

            // Открываем CSV-файл
            //OpenCsvFile("KnightTour.csv");
        }


        private async void button1_Click(object sender, EventArgs e)
        {
            StartTimer(); // Запускаем таймер

            string nText = sizeN.Text;
            string mText = sizeM.Text;

            if (int.TryParse(nText, out int n) && int.TryParse(mText, out int m))
            {
                panel.Controls.Clear();

                // Создаем TableLayoutPanel
                TableLayoutPanel tableLayoutPanel = new TableLayoutPanel
                {
                    RowCount = m,
                    ColumnCount = n,
                    Dock = DockStyle.Fill // Растягиваем таблицу на всю панель
                };

                // Устанавливаем процентное распределение для строк и столбцов
                for (int i = 0; i < m; i++)
                {
                    tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100f / m));
                }
                for (int j = 0; j < n; j++)
                {
                    tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / n));
                }

                textBoxes = new TextBox[m, n];
                visited = new bool[m, n];

                // Добавляем TextBox в каждую клетку
                for (int i = 0; i < m; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        TextBox textBox = new TextBox
                        {
                            Dock = DockStyle.Fill,
                            Multiline = true,
                            ReadOnly = true
                        };

                        // Чередуем цвета для клеток
                        textBox.BackColor = (i + j) % 2 == 0 ? Color.White : Color.Gray;

                        textBoxes[i, j] = textBox;
                        tableLayoutPanel.Controls.Add(textBox, j, i);
                    }
                }

                // Добавляем TableLayoutPanel на панель
                panel.Controls.Add(tableLayoutPanel);

                // Получаем стартовые координаты
                startX = int.TryParse(StartPointX.Text, out int parsedX) ? parsedX : 0;
                startY = int.TryParse(StartPointY.Text, out int parsedY) ? parsedY : 0;

                currentStep = 0;

                // Запускаем расчет пути коня в отдельном потоке
                List<Point> path = await Task.Run(() => FindKnightTour(n, m, new Point(startX, startY)));

                // Записываем данные в CSV файл
                string filePath = "KnightTour.csv";
                WriteStartInfoToCSV(filePath, startX, startY, n, m);
                if (path.Count > 0)
                {
                    WriteMovesToCSV(filePath, path);

                    // Проходим по найденному пути и выполняем шаги коня
                    foreach (var move in path)
                    {
                        await UpdateUI(move.X, move.Y);  // Обновляем UI для каждого хода
                        await Task.Delay(delay);         // Задержка между ходами
                    }
                }
                else
                {
                    MessageBox.Show("Нет решения для текущего стартового положения.");
                }
                StopTimer();
            }
            else
            {
                MessageBox.Show("Пожалуйста, введите корректные значения для N и M.");
            }
        }


        private Task UpdateUI(int x, int y)
        {
            // Обновляем интерфейс в UI-потоке
            return Task.Run(() =>
            {
                // Проверяем, находятся ли координаты в пределах массива
                if (x >= 0 && x < textBoxes.GetLength(1) && y >= 0 && y < textBoxes.GetLength(0)) // меняем x и y местами
                {
                    if (InvokeRequired)
                    {
                        Invoke(new Action(() =>
                        {
                            textBoxes[y, x].Text = currentStep.ToString(); // меняем местами x и y
                            visited[y, x] = true; // меняем местами x и y
                            currentStep++;
                        }));
                    }
                    else
                    {
                        textBoxes[y, x].Text = currentStep.ToString(); // меняем местами x и y
                        visited[y, x] = true; // меняем местами x и y
                        currentStep++;
                    }
                }
            });
        }

        // Новый метод для поиска тура коня
        private List<Point> FindKnightTour(int boardWidth, int boardHeight, Point start)
        {
            var path = new List<Point> { start };
            var visited = new bool[boardWidth, boardHeight];
            visited[start.X, start.Y] = true;

            Point? currentPos = start;

            while (path.Count < boardWidth * boardHeight)
            {
                Point? nextMove = GetNextMove(currentPos.Value, visited, boardWidth, boardHeight);

                if (nextMove == null)
                {
                    // Если нет доступных ходов, возвращаем пустой путь
                    return new List<Point>();
                }

                path.Add(nextMove.Value);
                visited[nextMove.Value.X, nextMove.Value.Y] = true;
                currentPos = nextMove;
            }

            return path;
        }

        private Point? GetNextMove(Point currentPos, bool[,] visited, int boardWidth, int boardHeight)
        {
            var moves = new List<Point>
    {
        new Point(2, 1), new Point(1, 2), new Point(-1, 2), new Point(-2, 1),
        new Point(-2, -1), new Point(-1, -2), new Point(1, -2), new Point(2, -1)
    };

            // Сортируем ходы по количеству доступных ходов для следующей позиции (правило Варнсдорфа)
            var nextMoves = new List<(Point move, int degree)>();

            foreach (var move in moves)
            {
                int newX = currentPos.X + move.X;
                int newY = currentPos.Y + move.Y;

                if (IsValid(newX, newY, boardWidth, boardHeight) && !visited[newX, newY])
                {
                    // Вычисляем количество доступных ходов из следующей позиции
                    int degree = GetMoveDegree(newX, newY, visited, boardWidth, boardHeight);
                    nextMoves.Add((new Point(newX, newY), degree));
                }
            }

            // Если нет доступных ходов, возвращаем null
            if (nextMoves.Count == 0)
                return null;

            // Сортируем по степени (количество доступных ходов из следующей позиции) и выбираем с минимальной степенью
            nextMoves.Sort((a, b) => a.degree.CompareTo(b.degree));

            return nextMoves.First().move;
        }

        // Метод для получения количества доступных ходов для позиции
        private int GetMoveDegree(int x, int y, bool[,] visited, int boardWidth, int boardHeight)
        {
            var moves = new List<Point>
    {
        new Point(2, 1), new Point(1, 2), new Point(-1, 2), new Point(-2, 1),
        new Point(-2, -1), new Point(-1, -2), new Point(1, -2), new Point(2, -1)
    };

            int count = 0;

            foreach (var move in moves)
            {
                int newX = x + move.X;
                int newY = y + move.Y;

                if (IsValid(newX, newY, boardWidth, boardHeight) && !visited[newX, newY])
                {
                    count++;
                }
            }

            return count;
        }

        // Проверка валидности хода
        private bool IsValid(int x, int y, int boardWidth, int boardHeight)
        {
            return x >= 0 && x < boardWidth && y >= 0 && y < boardHeight;
        }


        private async Task<bool> MakeKnightMove(int x, int y)
        {
            if (x < 0 || y < 0 || x >= textBoxes.GetLength(0) || y >= textBoxes.GetLength(1) || visited[x, y])
            {
                return false;
            }

            if (textBoxes[x, y].InvokeRequired)
            {
                textBoxes[x, y].Invoke(new Action(() => textBoxes[x, y].Text = currentStep.ToString()));
            }
            else
            {
                textBoxes[x, y].Text = currentStep.ToString();
            }

            visited[x, y] = true;

            currentStep++;

            await Task.Delay(delay);

            if (currentStep == textBoxes.Length)
            {
                return true;
            }

            for (int i = 0; i < 8; i++)
            {
                int newX = x + dx[i];
                int newY = y + dy[i];

                if (await MakeKnightMove(newX, newY))
                {
                    return true;
                }
            }

            visited[x, y] = false;

            if (textBoxes[x, y].InvokeRequired)
            {
                textBoxes[x, y].Invoke(new Action(() => textBoxes[x, y].Text = ""));
            }
            else
            {
                textBoxes[x, y].Text = "";
            }

            currentStep--;

            return false;
        }

    }
}
