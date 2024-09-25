using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Maze
{
    public partial class MainWindow : Window
    {
        private int[,] maze = {
            { 2, 0, 0, 0, 0, 0, 0, 0, 0, 3 },
            { 0, 1, 1, 0, 1, 1, 1, 1, 0, 1 },
            { 0, 0, 1, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 1, 1, 0, 1, 0, 1, 1, 1, 0 },
            { 0, 0, 0, 0, 1, 1, 1, 0, 1, 0 },
            { 1, 1, 1, 1, 0, 0, 1, 1, 0, 0 }
        };
        private int cellSize = 40;
        private List<List<Point>> allPaths = new List<List<Point>>();
        private int currentPathIndex = 0; // Индекс для переключения путей
        private bool isEditing = false; // Флаг режима редактирования

        public MainWindow()
        {
            InitializeComponent();
            DrawMaze();
        }

        // Метод для отрисовки лабиринта
        private void DrawMaze()
        {
            MazeCanvas.Children.Clear();

            for (int i = 0; i < maze.GetLength(0); i++)
            {
                for (int j = 0; j < maze.GetLength(1); j++)
                {
                    Rectangle rect = new Rectangle
                    {
                        Width = cellSize,
                        Height = cellSize,
                        Stroke = Brushes.Black,
                        Fill = maze[i, j] == 0 ? Brushes.White :
                               maze[i, j] == 1 ? Brushes.Black :
                               maze[i, j] == 2 ? Brushes.Blue : Brushes.Red
                    };
                    Canvas.SetLeft(rect, j * cellSize);
                    Canvas.SetTop(rect, i * cellSize);
                    MazeCanvas.Children.Add(rect);
                }
            }
        }

        // Очистка подсветки лабиринта
        private void ClearMaze()
        {
            foreach (var child in MazeCanvas.Children.OfType<Rectangle>())
            {
                if (child.Fill == Brushes.Yellow || child.Fill == Brushes.Green)
                {
                    child.Fill = Brushes.White;
                }
            }
        }

        // Поиск всех путей
        private void FindAllPaths_Click(object sender, RoutedEventArgs e)
        {
            ClearMaze();
            Point start = FindPoint(2); // Старт
            Point end = FindPoint(3);   // Финиш

            if (start == new Point(-1, -1) || end == new Point(-1, -1))
            {
                RouteInfo.Text = "Старт или конец не найдены!";
                return;
            }

            allPaths.Clear();
            List<Point> currentPath = new List<Point>();
            FindAllPaths(start, end, currentPath);

            if (allPaths.Count == 0)
            {
                RouteInfo.Text = "Пути не найдены!";
            }
            else
            {
                RouteInfo.Text = $"Найдено путей: {allPaths.Count}";
            }
        }

        // Поиск кратчайшего пути
        private void FindShortestPath_Click(object sender, RoutedEventArgs e)
        {
            if (allPaths.Count == 0)
            {
                RouteInfo.Text = "Сначала найдите все пути!";
                return;
            }

            allPaths = allPaths.OrderBy(p => p.Count).ToList(); // Сортировка по длине
            currentPathIndex = 0; // Устанавливаем на первый путь
            HighlightPath(allPaths[currentPathIndex], "Green");
            RouteInfo.Text = $"Показан кратчайший путь. Длина: {allPaths[0].Count}";
        }

        // Переключение на следующий путь
        private void ShowNextRoute_Click(object sender, RoutedEventArgs e)
        {
            if (allPaths.Count == 0)
            {
                RouteInfo.Text = "Сначала найдите все пути!";
                return;
            }

            ClearMaze();

            // Увеличиваем индекс и проверяем, чтобы не выйти за пределы
            currentPathIndex = (currentPathIndex + 1) % allPaths.Count;
            HighlightPath(allPaths[currentPathIndex], "Yellow");
            RouteInfo.Text = $"Путь {currentPathIndex + 1} из {allPaths.Count}";
        }

        // Метод для поиска всех возможных путей
        private void FindAllPaths(Point current, Point end, List<Point> currentPath)
        {
            currentPath.Add(current);

            if (current == end)
            {
                allPaths.Add(new List<Point>(currentPath));
            }
            else
            {
                var neighbors = GetNeighbors(new Node(current, null, 0, 0), maze.GetLength(0), maze.GetLength(1));

                foreach (var neighbor in neighbors)
                {
                    if (!currentPath.Contains(neighbor.Position))
                    {
                        FindAllPaths(neighbor.Position, end, currentPath);
                    }
                }
            }

            currentPath.RemoveAt(currentPath.Count - 1);
        }

        // Поиск точки в лабиринте
        private Point FindPoint(int value)
        {
            for (int i = 0; i < maze.GetLength(0); i++)
            {
                for (int j = 0; j < maze.GetLength(1); j++)
                {
                    if (maze[i, j] == value)
                        return new Point(j, i);
                }
            }
            return new Point(-1, -1);
        }

        // Подсветка пути
        private void HighlightPath(List<Point> path, string color)
        {
            foreach (var point in path)
            {
                var child = MazeCanvas.Children
                    .OfType<Rectangle>()
                    .FirstOrDefault(r => Canvas.GetLeft(r) == point.X * cellSize && Canvas.GetTop(r) == point.Y * cellSize);

                if (child != null && (child.Fill != Brushes.Blue && child.Fill != Brushes.Red))
                {
                    child.Fill = color == "Yellow" ? Brushes.Yellow : Brushes.Green;
                }
            }
        }

        // Реализация режима редактирования
        private void EditMaze_Click(object sender, RoutedEventArgs e)
        {
            isEditing = !isEditing;

            if (isEditing)
            {
                RouteInfo.Text = "Редактирование включено";
                AddClickHandlers();
            }
            else
            {
                RouteInfo.Text = "Редактирование завершено";
                RemoveClickHandlers();
            }
        }

        // Добавление обработчиков кликов для редактирования
        private void AddClickHandlers()
        {
            foreach (UIElement element in MazeCanvas.Children)
            {
                if (element is Rectangle rect)
                {
                    rect.MouseDown += Cell_MouseDown;
                }
            }

            AllPathsButton.Visibility = Visibility.Hidden;
            ShortestRouteButton.Visibility = Visibility.Hidden;
            NextRouteButton.Visibility = Visibility.Hidden;
            DoneButton.Visibility = Visibility.Visible;
        }

        // Удаление обработчиков кликов
        private void RemoveClickHandlers()
        {
            foreach (UIElement element in MazeCanvas.Children)
            {
                if (element is Rectangle rect)
                {
                    rect.MouseDown -= Cell_MouseDown;
                }
            }

            AllPathsButton.Visibility = Visibility.Visible;
            ShortestRouteButton.Visibility = Visibility.Visible;
            NextRouteButton.Visibility = Visibility.Visible;
            DoneButton.Visibility = Visibility.Collapsed;
        }
        private void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            ClearMaze();
            isEditing = false;
            RouteInfo.Text = "Редактирование завершено.";
            RemoveClickHandlers();
            //MainGrid.Children.Remove((Button)sender); // Удаляем кнопку "Готово"
        }

        private void Cell_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is Rectangle rect)
            {
                int x = (int)(Canvas.GetLeft(rect) / 40);
                int y = (int)(Canvas.GetTop(rect) / 40);

                if (maze[y, x] == 1) // Если это стена
                {
                    maze[y, x] = 0; // Изменяем на проход
                    rect.Fill = Brushes.White; // Меняем цвет на белый
                }
                else if (maze[y, x] == 0) // Если это проход
                {
                    maze[y, x] = 1; // Изменяем на стену
                    rect.Fill = Brushes.Black; // Меняем цвет на черный
                }

                Debug.WriteLine($"Ячейка изменена: ({x}, {y}) -> {maze[y, x]}");
            }
        }


        private IEnumerable<Node> GetNeighbors(Node node, int rows, int cols)
        {
            var neighbors = new List<Node>();
            var directions = new Point[]
            {
                new Point(0, -1), // Вверх
                new Point(0, 1),  // Вниз
                new Point(-1, 0), // Влево
                new Point(1, 0)   // Вправо
            };

            foreach (var direction in directions)
            {
                int newX = (int)node.Position.X + (int)direction.X;
                int newY = (int)node.Position.Y + (int)direction.Y;

                if (newX >= 0 && newX < cols && newY >= 0 && newY < rows && maze[newY, newX] != 1)
                {
                    neighbors.Add(new Node(new Point(newX, newY), null, 0, 0));
                }
            }

            return neighbors;
        }

        // Вспомогательный класс Node
        private class Node
        {
            public Point Position { get; set; }
            public Node Parent { get; set; }
            public int G { get; set; }
            public int H { get; set; }

            public Node(Point position, Node parent, int g, int h)
            {
                Position = position;
                Parent = parent;
                G = g;
                H = h;
            }
        }
    }
}
