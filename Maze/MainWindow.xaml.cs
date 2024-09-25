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
            { 0, 0, 0, 1, 0, 0, 1, 1, 0, 0 }
        };

        private List<Point> shortestPath;
        private List<Point> previousPath; // Хранит предыдущий путь
        private int currentPathIndex = -1; // Индекс текущего узла в пути

        public MainWindow()
        {
            InitializeComponent();

            Debug.WriteLine("MainWindow инициализирован.");
            DrawMaze();
        }

        private void DrawMaze()
        {
            MazeCanvas.Children.Clear();
            int cellSize = 40;

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

        private void ClearMaze()
        {
            int cellSize = 40;

            // Проходим по всем элементам на Canvas
            foreach (var child in MazeCanvas.Children.OfType<Rectangle>())
            {
                //// Проверяем, не является ли цвет текущей клетки цветом входа или выхода
                //if (child.Fill != Brushes.Blue || child.Fill != Brushes.Red || child.Fill != Brushes.Black)
                //{
                    
                //}
                if (child.Fill == Brushes.Yellow || child.Fill == Brushes.Green)
                {
                    child.Fill = Brushes.White; // Сбрасываем цвет на белый
                }
            }
        }


        private void FindShortestPath_Click(object sender, RoutedEventArgs e)
        {
            ClearMaze();
            Point start = FindPoint(2); // Стартовая точка (где 2)
            Point end = FindPoint(3);   // Конечная точка (где 3)

            Debug.WriteLine($"Стартовая точка: {start}");
            Debug.WriteLine($"Конечная точка: {end}");

            if (start == new Point(-1, -1) || end == new Point(-1, -1))
            {
                RouteInfo.Text = "Стартовая или конечная точка не найдена!";
                return;
            }

            shortestPath = FindPathAStar(start, end);
            if (shortestPath == null || shortestPath.Count == 0)
            {
                RouteInfo.Text = "Кратчайший путь не найден!";
            }
            else
            {
                RouteInfo.Text = $"Кратчайший путь найден! Длина: {shortestPath.Count}";
                HighlightPath(shortestPath, "Yellow");
            }
        }

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
            Debug.WriteLine($"Значение {value} не найдено.");
            Console.WriteLine($"Значение {value} не найдено.");
            DebugOutput.Text = $"Значение {value} не найдено.";

            return new Point(-1, -1); // Если точка не найдена
        }

        private void HighlightPath(List<Point> path, string color)
        {
            int cellSize = 40;
            foreach (var point in path)
            {
                var child = MazeCanvas.Children
                    .Cast<Rectangle>()
                    .FirstOrDefault(r => Canvas.GetLeft(r) == point.X * cellSize && Canvas.GetTop(r) == point.Y * cellSize);
                
                if (child != null && color == "Yellow" && (child.Fill != Brushes.Blue && child.Fill != Brushes.Red))
                {
                    child.Fill = Brushes.Yellow; // Подсвечиваем путь
                }
                else if (child != null && color == "Green")
                {
                    child.Fill = Brushes.Green; // Подсвечиваем путь
                }
            }
        }

        private List<Point> ReconstructPath(Node node)
        {
            var path = new List<Point>();
            while (node != null)
            {
                path.Add(node.Position);
                node = node.Parent;
            }
            path.Reverse();
            return path;
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
                    Debug.WriteLine($"Найден сосед: {newX}, {newY}");
                }
                else
                {
                    Debug.WriteLine($"Сосед не найден: {newX}, {newY}");
                }
            }

            return neighbors;
        }

        private int GetHeuristic(Point a, Point b)
        {
            int heuristic = Math.Abs((int)a.X - (int)b.X) + Math.Abs((int)a.Y - (int)b.Y);
            Debug.WriteLine($"Эвристика от {a} до {b}: {heuristic}");
            return heuristic; // Манхэттенское расстояние
        }

        private class Node
        {
            public Point Position { get; set; }
            public Node Parent { get; set; }
            public int G { get; set; } // Длина пути от начальной точки
            public int H { get; set; } // Оценка расстояния до цели
            public int F { get; set; } // G + H

            public Node(Point position, Node parent, int g, int h)
            {
                Position = position;
                Parent = parent;
                G = g;
                H = h;
                F = G + H;
            }
        }
        private bool isEditing = false;

        private void EditMaze_Click(object sender, RoutedEventArgs e)
        {
            isEditing = !isEditing; // Переключаем режим редактирования

            if (isEditing)
            {
                RouteInfo.Text = "Режим редактирования включен. Нажмите на клетки, чтобы изменить их.";
                AddClickHandlers();
            }
            else
            {
                RouteInfo.Text = "Режим редактирования выключен.";
                RemoveClickHandlers();
            }
        }

        private void AddClickHandlers()
        {
            foreach (UIElement element in MazeCanvas.Children)
            {
                if (element is Rectangle rect)
                {
                    rect.MouseDown += Cell_MouseDown;
                }
            }

            ShortestRouteButton.Visibility = Visibility.Hidden;
            NextRouteButton.Visibility = Visibility.Hidden;
            DoneButton.Visibility = Visibility.Visible;
        }


        private void RemoveClickHandlers()
        {
            foreach (UIElement element in MazeCanvas.Children)
            {
                if (element is Rectangle rect)
                {
                    rect.MouseDown -= Cell_MouseDown;
                }
            }

            ShortestRouteButton.Visibility = Visibility.Visible;
            NextRouteButton.Visibility = Visibility.Visible;
            DoneButton.Visibility = Visibility.Collapsed;
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
                    rect.Fill = Brushes.White; // Изменяем цвет на белый
                }
                else if (maze[y, x] == 0) // Если это проход
                {
                    maze[y, x] = 1; // Изменяем на стену
                    rect.Fill = Brushes.Black; // Изменяем цвет на черный
                }
            }
        }

        private void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            isEditing = false;
            RouteInfo.Text = "Редактирование завершено.";
            RemoveClickHandlers();
            //MainGrid.Children.Remove((Button)sender); // Удаляем кнопку "Готово"
        }

        private void ShowNextRoute_Click(object sender, RoutedEventArgs e)
        {
            if (shortestPath == null || shortestPath.Count == 0)
            {
                RouteInfo.Text = "Сначала найдите кратчайший путь!";
                return;
            }

            Point start = FindPoint(2);
            Point end = FindPoint(3);

            List<List<Point>> allPaths = new List<List<Point>> { shortestPath };
            List<Point> nextPath = null;

            do
            {
                nextPath = FindPathAStar(start, end, allPaths);
                if (nextPath != null)
                {
                    allPaths.Add(nextPath);
                    HighlightPath(nextPath, "Green"); // Визуализируем новый путь
                    return;
                }
            } while (nextPath != null);

            RouteInfo.Text = "Следующий путь не найден!";
        }


        private List<Point> FindPathAStar(Point start, Point end, List<List<Point>> previousPaths = null)
        {
            int rows = maze.GetLength(0);
            int cols = maze.GetLength(1);
            var openSet = new SortedSet<Node>(Comparer<Node>.Create((a, b) => a.F != b.F ? a.F.CompareTo(b.F) : a.Position.GetHashCode().CompareTo(b.Position.GetHashCode())));
            var closedSet = new HashSet<Node>();
            var startNode = new Node(start, null, 0, GetHeuristic(start, end));
            openSet.Add(startNode);

            Debug.WriteLine("Начинаем поиск пути...");

            while (openSet.Count > 0)
            {
                var current = openSet.Min; // Получаем узел с наименьшим F
                Debug.WriteLine($"Текущий узел: {current.Position} (F: {current.F})");

                if (current.Position == end)
                {
                    var newPath = ReconstructPath(current);

                    // Если переданы предыдущие пути, выполняем проверку
                    if (previousPaths != null && !ArePathsDifferent(newPath, previousPaths))
                    {
                        Debug.WriteLine("Путь совпадает с предыдущими путями, продолжаем поиск...");
                        openSet.Remove(current); // Удаляем текущий узел из openSet, чтобы избежать зацикливания
                        continue; // Пропускаем этот путь, если он совпадает
                    }

                    Debug.WriteLine("Достигнута конечная точка!");
                    return newPath; // Возвращаем новый путь
                }

                openSet.Remove(current);
                closedSet.Add(current);

                foreach (var neighbor in GetNeighbors(current, rows, cols))
                {
                    if (closedSet.Any(n => n.Position == neighbor.Position))
                        continue; // Пропускаем уже исследованные узлы

                    int tentativeG = current.G + 1;
                    var existingNode = openSet.FirstOrDefault(n => n.Position == neighbor.Position);

                    if (existingNode == null)
                    {
                        Debug.WriteLine($"Добавляем соседний узел: {neighbor.Position}");
                        openSet.Add(new Node(neighbor.Position, current, tentativeG, GetHeuristic(neighbor.Position, end)));
                    }
                    else if (tentativeG < existingNode.G)
                    {
                        // Обновляем узел, если найден более короткий путь
                        existingNode.Parent = current;
                        existingNode.G = tentativeG;
                        existingNode.F = tentativeG + existingNode.H;

                        // Удаляем и снова добавляем, чтобы сохранить порядок
                        openSet.Remove(existingNode);
                        openSet.Add(existingNode);
                    }
                }
            }

            Debug.WriteLine("Путь не найден.");
            return null; // Путь не найден
        }


        // Функция, проверяющая, отличается ли текущий путь от предыдущих
        private bool ArePathsDifferent(List<Point> newPath, List<List<Point>> previousPaths)
        {
            // Проверяем, есть ли совпадения с предыдущими путями
            foreach (var path in previousPaths)
            {
                // Сравниваем длину путей
                if (path.Count == newPath.Count)
                {
                    // Сравниваем каждую точку
                    bool allEqual = true;
                    for (int i = 0; i < path.Count; i++)
                    {
                        if (!path[i].Equals(newPath[i]))
                        {
                            allEqual = false; // Если хоть одна точка различается
                            break;
                        }
                    }
                    // Если все точки совпадают, пути одинаковые
                    if (allEqual)
                    {
                        return false; // Путь совпадает
                    }
                }
            }
            return true; // Путь отличается
        }


        private bool IsPathDifferent(List<Point> newPath, List<List<Point>> previousPaths)
        {
            foreach (var path in previousPaths)
            {
                if (newPath.Count == path.Count && newPath.SequenceEqual(path))
                {
                    return false; // Путь совпадает с одним из предыдущих
                }
            }
            return true; // Путь отличается
        }

    }
}
