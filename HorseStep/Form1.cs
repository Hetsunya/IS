using System.Diagnostics;

namespace HorseStep
{
    public partial class Form1 : Form
    {
        private readonly int[] dx = { 2, 2, 1, 1, -1, -1, -2, -2 }; // ��������� �������� �� x
        private readonly int[] dy = { 1, -1, 2, -2, 2, -2, 1, -1 }; // ��������� �������� �� y
        private TextBox[,] textBoxes; // ������ ��� �������� ������ �� ��������� ����
        private bool[,] visited; // ������ ��� �������� ����������, ���������� �� ������
        private int currentStep = 0;  // ����� �������� ����
        private int startX, startY;    // ��������� ���������� ����
        private int delay = 200;  // �������� ����� ������ (�� ���������)
        private bool timerRunning = false;
        private double elapsedTime = 0; // ������� ������� � ��������

        public Form1()
        {
            InitializeComponent();

            // ������������� �������� �������� ��������
            delayTrackBar.Minimum = 0;
            delayTrackBar.Maximum = 1000;
            delayTrackBar.Value = delay; // ��������� ��������
            delayTrackBar.TickFrequency = 10; // ��� ���������

            // ��������� ���������� ��� ��������� �������� ��������
            delayTrackBar.Scroll += new EventHandler(delayTrackBar_Scroll);

            this.Text = "������ � ���� ����";
        }


        private void delayTrackBar_Scroll(object sender, EventArgs e)
        {
            delay = delayTrackBar.Value;
        }

        private void OpenCsvFile(string filePath)
        {
            try
            {
                // ������ ����� ������� ��� �������� �����
                Process.Start(new ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"�� ������� ������� ����: {ex.Message}");
            }
        }


        private void WriteStartInfoToCSV(string filePath, int startX, int startY, int boardWidth, int boardHeight)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("������; ��������� ���������� X; ��������� ���������� Y; ������; ������");
                writer.WriteLine($"������; {startX}; {startY}; {boardWidth}; {boardHeight}");
            }
        }

        private void WriteMovesToCSV(string filePath, List<Point> path)
        {
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine("���; X; Y");
                for (int i = 0; i < path.Count; i++)
                {
                    writer.WriteLine($"{i}; {path[i].X + 1}; {path[i].Y + 1}");
                }
            }
        }


        // ����� ��� ���������� �������
        private async void StartTimer()
        {
            timerRunning = true;
            elapsedTime = 0;

            while (timerRunning)
            {
                await Task.Delay(10); // �������� �� 100 ����������� ��� �������� �� ������� �����
                elapsedTime += 0.01; // ����������� ����� �� 0.1 ������� (100 ��)

                // ��������� ��������� ����� UI-�����
                if (InvokeRequired)
                {
                    Invoke(new Action(() =>
                    {
                        // ����������� ����� � ����� ������� ����� �������
                        timeLabel.Text = $"������ �������: {elapsedTime:F2} ���";
                    }));
                }
                else
                {
                    timeLabel.Text = $"������ �������: {elapsedTime:F2} ���";
                }
            }
        }

        private void StopTimer()
        {
            timerRunning = false;

            // ��������� CSV-����
            //OpenCsvFile("KnightTour.csv");
        }


        private async void button1_Click(object sender, EventArgs e)
        {
            StartTimer(); // ��������� ������

            string nText = sizeN.Text;
            string mText = sizeM.Text;

            if (int.TryParse(nText, out int n) && int.TryParse(mText, out int m))
            {
                panel.Controls.Clear();

                // ������� TableLayoutPanel
                TableLayoutPanel tableLayoutPanel = new TableLayoutPanel
                {
                    RowCount = m,
                    ColumnCount = n,
                    Dock = DockStyle.Fill // ����������� ������� �� ��� ������
                };

                // ������������� ���������� ������������� ��� ����� � ��������
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

                // ��������� TextBox � ������ ������
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

                        // �������� ����� ��� ������
                        textBox.BackColor = (i + j) % 2 == 0 ? Color.White : Color.Gray;

                        textBoxes[i, j] = textBox;
                        tableLayoutPanel.Controls.Add(textBox, j, i);
                    }
                }

                // ��������� TableLayoutPanel �� ������
                panel.Controls.Add(tableLayoutPanel);

                // �������� ��������� ����������
                startX = int.TryParse(StartPointX.Text, out int parsedX) ? parsedX : 0;
                startY = int.TryParse(StartPointY.Text, out int parsedY) ? parsedY : 0;

                currentStep = 0;

                // ��������� ������ ���� ���� � ��������� ������
                List<Point> path = await Task.Run(() => FindKnightTour(n, m, new Point(startX, startY)));

                // ���������� ������ � CSV ����
                string filePath = "KnightTour.csv";
                WriteStartInfoToCSV(filePath, startX, startY, n, m);
                if (path.Count > 0)
                {
                    WriteMovesToCSV(filePath, path);

                    // �������� �� ���������� ���� � ��������� ���� ����
                    foreach (var move in path)
                    {
                        await UpdateUI(move.X, move.Y);  // ��������� UI ��� ������� ����
                        await Task.Delay(delay);         // �������� ����� ������
                    }
                }
                else
                {
                    MessageBox.Show("��� ������� ��� �������� ���������� ���������.");
                }
                StopTimer();
            }
            else
            {
                MessageBox.Show("����������, ������� ���������� �������� ��� N � M.");
            }
        }


        private Task UpdateUI(int x, int y)
        {
            // ��������� ��������� � UI-������
            return Task.Run(() =>
            {
                // ���������, ��������� �� ���������� � �������� �������
                if (x >= 0 && x < textBoxes.GetLength(1) && y >= 0 && y < textBoxes.GetLength(0)) // ������ x � y �������
                {
                    if (InvokeRequired)
                    {
                        Invoke(new Action(() =>
                        {
                            textBoxes[y, x].Text = currentStep.ToString(); // ������ ������� x � y
                            visited[y, x] = true; // ������ ������� x � y
                            currentStep++;
                        }));
                    }
                    else
                    {
                        textBoxes[y, x].Text = currentStep.ToString(); // ������ ������� x � y
                        visited[y, x] = true; // ������ ������� x � y
                        currentStep++;
                    }
                }
            });
        }

        // ����� ����� ��� ������ ���� ����
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
                    // ���� ��� ��������� �����, ���������� ������ ����
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

            // ��������� ���� �� ���������� ��������� ����� ��� ��������� ������� (������� ����������)
            var nextMoves = new List<(Point move, int degree)>();

            foreach (var move in moves)
            {
                int newX = currentPos.X + move.X;
                int newY = currentPos.Y + move.Y;

                if (IsValid(newX, newY, boardWidth, boardHeight) && !visited[newX, newY])
                {
                    // ��������� ���������� ��������� ����� �� ��������� �������
                    int degree = GetMoveDegree(newX, newY, visited, boardWidth, boardHeight);
                    nextMoves.Add((new Point(newX, newY), degree));
                }
            }

            // ���� ��� ��������� �����, ���������� null
            if (nextMoves.Count == 0)
                return null;

            // ��������� �� ������� (���������� ��������� ����� �� ��������� �������) � �������� � ����������� ��������
            nextMoves.Sort((a, b) => a.degree.CompareTo(b.degree));

            return nextMoves.First().move;
        }

        // ����� ��� ��������� ���������� ��������� ����� ��� �������
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

        // �������� ���������� ����
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
