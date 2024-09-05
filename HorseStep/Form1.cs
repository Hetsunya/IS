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

        public Form1()
        {
            InitializeComponent();

            // ������������� �������� �������� ��������
            delayTrackBar.Minimum = 0;
            delayTrackBar.Maximum = 1000;
            delayTrackBar.Value = delay; // ��������� ��������
            delayTrackBar.TickFrequency = 100; // ��� ���������

            // ��������� ���������� ��� ��������� �������� ��������
            delayTrackBar.Scroll += new EventHandler(delayTrackBar_Scroll);
        }

        private void ClearResources()
        {
            // ������� ��� �������� �� ������
            panel.Controls.Clear();

            // �������� �������
            textBoxes = null;
            visited = null;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            ClearResources(); // ����������� ������ �������

            string nText = sizeN.Text;
            string mText = sizeM.Text;

            if (int.TryParse(nText, out int n) && int.TryParse(mText, out int m))
            {
                panel.Controls.Clear();

                // ������� TableLayoutPanel
                TableLayoutPanel tableLayoutPanel = new TableLayoutPanel();
                tableLayoutPanel.RowCount = m;
                tableLayoutPanel.ColumnCount = n;

                // ����������, ��� ����� ������������ ������ ����� (������ ��� ������)
                int cellSize = Math.Min(panel.Width / n, panel.Height / m); // ���������� ������ ������ ��� ������� �� ������ ��� ������

                // ������ ������� ������ �� ������ ������
                tableLayoutPanel.Width = cellSize * n;
                tableLayoutPanel.Height = cellSize * m;
                tableLayoutPanel.Dock = DockStyle.None; // ��������� �������������� ��������� �������

                // ������������� ���������� ������������� ��� ����� � ��������
                for (int i = 0; i < m; i++)
                {
                    tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, cellSize));
                }
                for (int j = 0; j < n; j++)
                {
                    tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, cellSize));
                }

                textBoxes = new TextBox[m, n];
                visited = new bool[m, n];

                // ��������� TextBox � ������ ������
                for (int i = 0; i < m; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        TextBox textBox = new TextBox();
                        textBox.Dock = DockStyle.Fill;
                        textBox.Multiline = true;
                        textBox.ReadOnly = true;

                        if ((i + j) % 2 == 0)
                        {
                            textBox.BackColor = Color.White;
                        }
                        else
                        {
                            textBox.BackColor = Color.Gray;
                        }

                        textBoxes[i, j] = textBox;
                        tableLayoutPanel.Controls.Add(textBox, j, i);
                    }
                }

                // ��������� TableLayoutPanel �� ������
                panel.Controls.Add(tableLayoutPanel);
                tableLayoutPanel.Left = (panel.Width - tableLayoutPanel.Width) / 2;  // ���������� �� �����������
                tableLayoutPanel.Top = (panel.Height - tableLayoutPanel.Height) / 2; // ���������� �� ���������

                // �������� ��������� ����������
                startX = int.TryParse(StartPointX.Text, out int parsedX) ? parsedX : 0;
                startY = int.TryParse(StartPointY.Text, out int parsedY) ? parsedY : 0;

                currentStep = 0;

                // ��������� ������ ���� ���� � ��������� ������
                List<Point> path = await Task.Run(() => FindKnightTour(n, m, new Point(startX, startY)));

                if (path.Count > 0)
                {
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
            var moves = new List<Point>
            {
                new Point(2, 1), new Point(1, 2), new Point(-1, 2), new Point(-2, 1),
                new Point(-2, -1), new Point(-1, -2), new Point(1, -2), new Point(2, -1)
            };

            bool IsValid(int x, int y) => x >= 0 && x < boardWidth && y >= 0 && y < boardHeight;

            bool DFS(Point pos, List<Point> path, bool[,] visited)
            {
                if (path.Count == boardWidth * boardHeight)
                    return true;

                foreach (var move in moves)
                {
                    var newPos = new Point(pos.X + move.X, pos.Y + move.Y);
                    if (IsValid(newPos.X, newPos.Y) && !visited[newPos.X, newPos.Y])
                    {
                        visited[newPos.X, newPos.Y] = true;
                        path.Add(newPos);
                        if (DFS(newPos, path, visited))
                            return true;
                        path.RemoveAt(path.Count - 1);
                        visited[newPos.X, newPos.Y] = false;
                    }
                }

                return false;
            }

            var path = new List<Point> { start };
            var visited = new bool[boardWidth, boardHeight];
            visited[start.X, start.Y] = true;

            if (DFS(start, path, visited))
            {
                return path;
            }
            else
            {
                return new List<Point>();
            }
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

        private void delayTrackBar_Scroll(object sender, EventArgs e)
        {
            delay = delayTrackBar.Value;
        }
    }
}
