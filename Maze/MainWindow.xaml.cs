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

            //foreach (var child in MazeCanvas.Children.Cast<Rectangle>)
            //{
            //    child
            //}
            // Проходим по всем элементам на Canvas
            for (int i = 0; i < maze.GetLength(0); i++)
            {
                for (int j = 0; j < maze.GetLength(1); j++)
                {
                    // Находим прямоугольник, соответствующий клетке в лабиринте
                    var child = MazeCanvas.Children
                        .Cast<Rectangle>()
                        .FirstOrDefault(r => Canvas.GetLeft(r) == j * cellSize && Canvas.GetTop(r) == i * cellSize);


                    Debug.WriteLine($"Залупа дитя {child}");
                    if (child != null && child.Fill == Brushes.Yellow)
                    {
                        Debug.WriteLine ($"Залупа дитя {child}");
                        child.Fill = Brushes.White;
                    }
                    else if (child != null && child.Fill == Brushes.Green)
                    {

                        Debug.WriteLine($"Залупа дитя {child}");
                        child.Fill = Brushes.White;
                    }
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

                if (child != null && color == "Yellow")
                {
                    child.Fill = Brushes.Yellow; // Подсвечиваем путь
                }
                else if (child != null && color == "Green")
                {
                    child.Fill = Brushes.Green; // Подсвечиваем путь
                }
            }
        }


        private List<Point> FindPathAStar(Point start, Point end)
        {
            int rows = maze.GetLength(0);
            int cols = maze.GetLength(1);
            var openSet = new List<Node>();
            var closedSet = new HashSet<Node>();
            var startNode = new Node(start, null, 0, GetHeuristic(start, end));
            openSet.Add(startNode);

            Debug.WriteLine("Начинаем поиск пути...");

            while (openSet.Count > 0)
            {
                var current = openSet.OrderBy(node => node.F).First();
                Debug.WriteLine($"Текущий узел: {current.Position} (F: {current.F})");

                if (current.Position == end)
                {
                    Debug.WriteLine("Достигнута конечная точка!");
                    return ReconstructPath(current);
                }

                openSet.Remove(current);
                closedSet.Add(current);

                foreach (var neighbor in GetNeighbors(current, rows, cols))
                {
                    if (closedSet.Any(n => n.Position == neighbor.Position))
                        continue;

                    int tentativeG = current.G + 1;
                    var existingNode = openSet.FirstOrDefault(n => n.Position == neighbor.Position);

                    if (existingNode == null)
                    {
                        Debug.WriteLine($"Добавляем соседний узел: {neighbor.Position}");
                        openSet.Add(new Node(neighbor.Position, current, tentativeG, GetHeuristic(neighbor.Position, end)));
                    }
                    else if (tentativeG < existingNode.G)
                    {
                        existingNode.Parent = current;
                        existingNode.G = tentativeG;
                        existingNode.F = tentativeG + existingNode.H;
                    }
                }
            }

            Debug.WriteLine("Путь не найден.");
            return null; // Путь не найден
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
                nextPath = FindNextPathAStarExcludingPrevious(start, end, allPaths);
                if (nextPath != null)
                {
                    allPaths.Add(nextPath);
                    break;
                }
            } while (nextPath != null);

            if (nextPath == null || nextPath.Count == 0)
            {
                RouteInfo.Text = "Следующий путь не найден!";
                return;
            }

            HighlightPath(nextPath, "Green");
        }


        private List<Point> FindNextPathAStarExcludingPrevious(Point start, Point end, List<List<Point>> previousPaths)
        {
            int rows = maze.GetLength(0);
            int cols = maze.GetLength(1);
            var openSet = new List<Node>();
            var closedSet = new HashSet<Node>();
            var startNode = new Node(start, null, 0, GetHeuristic(start, end));
            openSet.Add(startNode);

            Debug.WriteLine("Начинаем поиск альтернативного пути...");

            while (openSet.Count > 0)
            {
                var current = openSet.OrderBy(node => node.F).First();

                if (current.Position == end)
                {
                    var newPath = ReconstructPath(current);

                    // Проверяем, пересекается ли новый путь с предыдущими
                    if (ArePathsDifferent(newPath, previousPaths))
                    {
                        return newPath;
                    }
                }

                openSet.Remove(current);
                closedSet.Add(current);

                foreach (var neighbor in GetNeighbors(current, rows, cols))
                {
                    if (closedSet.Any(n => n.Position == neighbor.Position))
                        continue;

                    int tentativeG = current.G + 1;
                    var existingNode = openSet.FirstOrDefault(n => n.Position == neighbor.Position);

                    if (existingNode == null)
                    {
                        openSet.Add(new Node(neighbor.Position, current, tentativeG, GetHeuristic(neighbor.Position, end)));
                    }
                    else if (tentativeG < existingNode.G)
                    {
                        existingNode.Parent = current;
                        existingNode.G = tentativeG;
                        existingNode.F = tentativeG + existingNode.H;
                    }
                }
            }

            return null; // Путь не найден
        }

        // Функция, проверяющая, отличается ли текущий путь от предыдущих
        private bool ArePathsDifferent(List<Point> newPath, List<List<Point>> previousPaths)
        {
            foreach (var path in previousPaths)
            {
                // Проверяем, есть ли хотя бы одна точка, которая не совпадает
                if (newPath.Any(p => path.Contains(p)))
                {
                    return false; // Путь пересекается с предыдущим
                }
            }
            return true; // Все пути различны
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
