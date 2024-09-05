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
                // ������� ���������� ��������
                panel.Controls.Clear();

                // ������� TableLayoutPanel
                TableLayoutPanel tableLayoutPanel = new TableLayoutPanel();
                tableLayoutPanel.RowCount = m;
                tableLayoutPanel.ColumnCount = n;
                tableLayoutPanel.Dock = DockStyle.Fill;

                // ������������� ���������� ������ ��� ���� ����� � ��������
                for (int i = 0; i < m; i++)
                {
                    tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100f / m));
                }
                for (int j = 0; j < n; j++)
                {
                    tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / n));
                }

                // �������������� ������ ��������� �����
                textBoxes = new TextBox[m, n];
                visited = new bool[m, n];

                // ��������� TableLayoutPanel ���������� ������ � ��������� ������ �� ���
                for (int i = 0; i < m; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        TextBox textBox = new TextBox();
                        textBox.Dock = DockStyle.Fill;
                        textBox.Multiline = true;
                        textBox.ReadOnly = true;

                        // ��������� ��������� �����
                        if ((i + j) % 2 == 0)
                        {
                            textBox.BackColor = Color.White;  // ����� ������
                        }
                        else
                        {
                            textBox.BackColor = Color.Gray;   // ������ ������
                        }

                        textBoxes[i, j] = textBox;
                        tableLayoutPanel.Controls.Add(textBox, j, i);
                    }
                }


                // ��������� TableLayoutPanel � panel2
                panel.Controls.Add(tableLayoutPanel);

                // ��������� ���������� ���� (����� ������ ����� ���������������� ����)
                startX = int.TryParse(StartPointX.Text, out int parsedX) ? parsedX : 0;
                startY = int.TryParse(StartPointY.Text, out int parsedY) ? parsedY : 0;

                // ���������� ����� ����
                currentStep = 0;

                // ���������� ��������� ���
                MakeKnightMove(startX, startY);
            }
            else
            {
                MessageBox.Show("����������, ������� ���������� �������� ��� N � M.");
            }
        }

        private async Task<bool> MakeKnightMove(int x, int y)
        {
            // ��������� ����� �� ������� ����� ��� ���� ������ ��� ��������
            if (x < 0 || y < 0 || x >= textBoxes.GetLength(0) || y >= textBoxes.GetLength(1) || visited[x, y])
            {
                return false;
            }

            // ���������� ����� ���� � ��������� ���� ����� Invoke
            if (textBoxes[x, y].InvokeRequired)
            {
                textBoxes[x, y].Invoke(new Action(() => textBoxes[x, y].Text = currentStep.ToString()));
            }
            else
            {
                textBoxes[x, y].Text = currentStep.ToString();
            }

            // �������� ������ ��� ����������
            visited[x, y] = true;

            // ��������� ����� ����
            currentStep++;

            // ��������� �������� ��� ���������� ����������, ��������� ������� �������� ��������
            await Task.Delay(delay);

            // ���� ��� ������ ��������, ���������� true
            if (currentStep == textBoxes.Length)
            {
                return true;
            }

            // ������� ��� 8 ��������� �����
            for (int i = 0; i < 8; i++)
            {
                int newX = x + dx[i];
                int newY = y + dy[i];

                if (await MakeKnightMove(newX, newY))
                {
                    return true;
                }
            }

            // ���� �� ���� ��� �� ��������, ���������� ��� � �������� ������ ��� ������������
            visited[x, y] = false;

            // ������� ��������� ���� ��� ��������
            if (textBoxes[x, y].InvokeRequired)
            {
                textBoxes[x, y].Invoke(new Action(() => textBoxes[x, y].Text = ""));
            }
            else
            {
                textBoxes[x, y].Text = "";
            }

            // ��������� ����� ���� (�� �� ���������� ���)
            currentStep--;

            return false;
        }


private (int, int) FindNextMove(int x, int y)
        {
            List<(int, int)> possibleMoves = new List<(int, int)>();

            // ����� ���� ��������� �����
            for (int i = 0; i < 8; i++)
            {
                int newX = x + dx[i];
                int newY = y + dy[i];

                if (IsValidMove(newX, newY))
                {
                    possibleMoves.Add((newX, newY));
                }
            }

            // �������� ���, ������� ����� �� ������ � ����������� ����������� ��������� ��������� �����
            var nextMove = possibleMoves
                .OrderBy(move => CountAvailableMoves(move.Item1, move.Item2))
                .FirstOrDefault();

            return nextMove != default ? nextMove : (-1, -1); // ���� ����� ���, ���������� (-1, -1)
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
            delay = delayTrackBar.Value; // ������������� �������� �� �������� ��������
        }
    }
}
