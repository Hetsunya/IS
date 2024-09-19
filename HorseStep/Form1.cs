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

            delayTrackBar.Minimum = 0;
            delayTrackBar.Maximum = 1000;
            delayTrackBar.Value = delay;
            delayTrackBar.TickFrequency = 10;
            delayTrackBar.Scroll += (s, e) => delay = delayTrackBar.Value;

            this.Text = "Задача о ходе коня";
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(sizeN.Text, out int n) || !int.TryParse(sizeM.Text, out int m))
            {
                MessageBox.Show("Пожалуйста, введите корректные значения для N и M.");
                return;
            }

            InitializeBoard(n, m);

            if (!int.TryParse(StartPointX.Text, out startX) || !int.TryParse(StartPointY.Text, out startY))
            {
                startX = startY = 0;
            }

            currentStep = 0;
            List<Point> path = await Task.Run(() => FindKnightTour(n, m, new Point(startX, startY)));

            if (path.Count > 0)
            {
                foreach (var move in path)
                {
                    await UpdateUI(move.X, move.Y);
                    await Task.Delay(delay);
                }
            }
            else
            {
                MessageBox.Show("Нет решения для текущего стартового положения.");
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

        private Task UpdateUI(int x, int y)
        {
            return Task.Run(() =>
            {
                if (IsValidPosition(x, y))
                {
                    Invoke(new Action(() =>
                    {
                        textBoxes[y, x].Text = currentStep.ToString();
                        visited[y, x] = true;
                        currentStep++;
                    }));
                }
            });
        }

        private List<Point> FindKnightTour(int boardWidth, int boardHeight, Point start)
        {
            var path = new List<Point> { start };
            var visited = new bool[boardWidth, boardHeight];
            visited[start.X, start.Y] = true;

            while (path.Count < boardWidth * boardHeight)
            {
                var nextMove = GetNextMove(path.Last(), visited, boardWidth, boardHeight);
                if (nextMove == null)
                    return new List<Point>(); // Нет решения

                path.Add(nextMove.Value);
                visited[nextMove.Value.X, nextMove.Value.Y] = true;
            }

            return path;
        }

        private Point? GetNextMove(Point currentPos, bool[,] visited, int boardWidth, int boardHeight)
        {
            var moves = GetPotentialMoves(currentPos);
            var nextMoves = moves
                .Where(move => IsValid(move.X, move.Y, boardWidth, boardHeight) && !visited[move.X, move.Y])
                .Select(move => (move, degree: GetMoveDegree(move.X, move.Y, visited, boardWidth, boardHeight)))
                .OrderBy(x => x.degree)
                .ToList();

            return nextMoves.FirstOrDefault().move;
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
