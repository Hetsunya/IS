namespace HorseStep
{
    public partial class Form1 : Form
    {
        private readonly int[] dx = { 2, 2, 1, 1, -1, -1, -2, -2 };
        private readonly int[] dy = { 1, -1, 2, -2, 2, -2, 1, -1 };
        private TextBox[,] textBoxes;
        private bool[,] visited;
        private int currentStep = 0;
        private int startX, startY;
        private int delay = 0;

        public Form1()
        {
            InitializeComponent();
            delayTrackBar.Minimum = 0;
            delayTrackBar.Maximum = 1000;
            delayTrackBar.Value = delay;
            delayTrackBar.TickFrequency = 10;
            delayTrackBar.Scroll += (s, e) => delay = delayTrackBar.Value;

            this.Text = "Задача о ходе коня";
        }

        private bool IsBoardValid(int width, int height)
        {
            // Обменяем значения, чтобы ширина всегда была <= высоты
            if (width > height) (width, height) = (height, width);

            // Правила для доски 3×n
            if (width == 3 && (height < 4 || (height > 4 && height < 7)))
                return false;

            // Исключение для доски 4×4
            if (width == 4 && height == 4)
                return false;

            // Во всех других случаях доска допустима
            return true;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(sizeN.Text, out int n) || !int.TryParse(sizeM.Text, out int m))
            {
                MessageBox.Show("Пожалуйста, введите корректные значения для N и M.");
                return;
            }

            // Проверка невозможных размеров доски
            if (!IsBoardValid(n, m))
            {
                MessageBox.Show("Обход коня невозможен с текущими размерами доски. " +
                                "Если одна сторона равна 3, другая должна быть 4 или не меньше 7. " +
                                "На доске 4×4 решение также невозможно.");
                return;
            }

            if (!int.TryParse(StartPointX.Text, out startX) || !int.TryParse(StartPointY.Text, out startY) ||
                !IsValid(startX, startY, n, m))
            {
                MessageBox.Show("Стартовая точка вне допустимых пределов доски.");
                return;
            }

            InitializeBoard(n, m);
            currentStep = 0;

            List<Point> path = await Task.Run(() => FindKnightTour(n, m, new Point(startX, startY)));

            if (path.Count > 0)
            {
                MessageBox.Show($"Решение найдено. Количество шагов: {path.Count}");
                foreach (var move in path)
                {
                    await UpdateUI(move.X, move.Y);
                }
            }
            else
            {
                MessageBox.Show("Решение не найдено! Попробуйте выбрать другую стартовую позицию.");
            }
        }



        private void InitializeBoard(int n, int m)
        {
            panel.Controls.Clear();
            var tableLayoutPanel = new TableLayoutPanel
            {
                RowCount = m,
                ColumnCount = n,
                Dock = DockStyle.Fill
            };

            for (int i = 0; i < m; i++)
                tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100f / m));
            for (int j = 0; j < n; j++)
                tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / n));

            textBoxes = new TextBox[m, n];
            visited = new bool[m, n];

            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    var textBox = new TextBox
                    {
                        Dock = DockStyle.Fill,
                        Multiline = true,
                        ReadOnly = true,
                        BackColor = (i + j) % 2 == 0 ? Color.White : Color.Gray
                    };
                    textBoxes[i, j] = textBox;
                    tableLayoutPanel.Controls.Add(textBox, j, i);
                }
            }

            panel.Controls.Add(tableLayoutPanel);
        }

        private async Task UpdateUI(int x, int y)
        {
            if (IsValidPosition(x, y))
            {
                textBoxes[y, x].Text = currentStep.ToString();
                visited[y, x] = true;
                currentStep++;
                await Task.Delay(delay);
            }
        }

        private List<Point> FindKnightTour(int boardWidth, int boardHeight, Point start)
        {
            var visited = new bool[boardWidth, boardHeight];
            visited[start.X, start.Y] = true;

            var path = new List<Point> { start };

            // Если изначально невозможно пройти весь маршрут
            if (Solve(boardWidth, boardHeight, start, path, visited))
            {
                return path;
            }

            return new List<Point>();
        }


        private bool Solve(int boardWidth, int boardHeight, Point currentPos, List<Point> path, bool[,] visited)
        {
            if (path.Count == boardWidth * boardHeight) return true;

            var nextMoves = GetPotentialMoves(currentPos)
                .Where(move => IsValid(move.X, move.Y, boardWidth, boardHeight) && !visited[move.X, move.Y])
                .OrderBy(move => GetMoveDegree(move.X, move.Y, visited, boardWidth, boardHeight))
                .ToList();

            foreach (var move in nextMoves)
            {
                visited[move.X, move.Y] = true;
                path.Add(move);

                if (Solve(boardWidth, boardHeight, move, path, visited)) return true;

                visited[move.X, move.Y] = false;
                path.RemoveAt(path.Count - 1);
            }

            return false;
        }

        private int GetMoveDegree(int x, int y, bool[,] visited, int boardWidth, int boardHeight)
        {
            return GetPotentialMoves(new Point(x, y))
                .Count(move => IsValid(move.X, move.Y, boardWidth, boardHeight) && !visited[move.X, move.Y]);
        }

        private List<Point> GetPotentialMoves(Point currentPos)
        {
            return dx.Zip(dy, (dx, dy) => new Point(currentPos.X + dx, currentPos.Y + dy)).ToList();
        }

        private bool IsValidPosition(int x, int y)
        {
            return x >= 0 && x < textBoxes.GetLength(1) && y >= 0 && y < textBoxes.GetLength(0);
        }

        private bool IsValid(int x, int y, int boardWidth, int boardHeight)
        {
            return x >= 0 && x < boardWidth && y >= 0 && y < boardHeight;
        }
    }
}
