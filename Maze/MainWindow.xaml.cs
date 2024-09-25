using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Diagnostics;

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
        private int currentPathIndex = 0;
        private bool isEditing = false;

        public MainWindow()
        {
            InitializeComponent();
            DrawMaze();
        }

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
                        Fill = maze[i, j] switch
                        {
                            0 => Brushes.White,
                            1 => Brushes.Black,
                            2 => Brushes.Blue,
                            _ => Brushes.Red
                        }
                    };
                    Canvas.SetLeft(rect, j * cellSize);
                    Canvas.SetTop(rect, i * cellSize);
                    MazeCanvas.Children.Add(rect);
                }
            }
        }

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

        private void FindAllPaths_Click(object sender, RoutedEventArgs e)
        {
            ClearMaze();
            Point start = FindPoint(2);
            Point end = FindPoint(3);

            if (start == new Point(-1, -1) || end == new Point(-1, -1))
            {
                RouteInfo.Text = "Старт или конец не найдены!";
                return;
            }

            allPaths.Clear();
            FindAllPaths(start, end, new List<Point>());

            RouteInfo.Text = allPaths.Count == 0 ? "Пути не найдены!" : $"Найдено путей: {allPaths.Count}";
        }

        private void FindShortestPath_Click(object sender, RoutedEventArgs e)
        {
            if (allPaths.Count == 0)
            {
                RouteInfo.Text = "Сначала найдите все пути!";
                return;
            }

            allPaths = allPaths.OrderBy(p => p.Count).ToList();
            currentPathIndex = 0;
            HighlightPath(allPaths[currentPathIndex], "Green");
            RouteInfo.Text = $"Показан кратчайший путь. Длина: {allPaths[0].Count}";
        }

        private void ShowNextRoute_Click(object sender, RoutedEventArgs e)
        {
            if (allPaths.Count == 0)
            {
                RouteInfo.Text = "Сначала найдите все пути!";
                return;
            }

            ClearMaze();
            currentPathIndex = (currentPathIndex + 1) % allPaths.Count;
            HighlightPath(allPaths[currentPathIndex], "Yellow");
            RouteInfo.Text = $"Путь {currentPathIndex + 1} из {allPaths.Count}";
        }

        private void FindAllPaths(Point current, Point end, List<Point> currentPath)
        {
            currentPath.Add(current);

            if (current == end)
            {
                allPaths.Add(new List<Point>(currentPath));
            }
            else
            {
                foreach (var neighbor in GetNeighbors(new Node(current), maze.GetLength(0), maze.GetLength(1)))
                {
                    if (!currentPath.Contains(neighbor.Position))
                    {
                        FindAllPaths(neighbor.Position, end, currentPath);
                    }
                }
            }

            currentPath.RemoveAt(currentPath.Count - 1);
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
            return new Point(-1, -1);
        }

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

        private void EditMaze_Click(object sender, RoutedEventArgs e)
        {
            isEditing = !isEditing;

            RouteInfo.Text = isEditing ? "Редактирование включено" : "Редактирование завершено";
            if (isEditing) AddClickHandlers(); else RemoveClickHandlers();
        }

        private void AddClickHandlers()
        {
            foreach (Rectangle rect in MazeCanvas.Children.OfType<Rectangle>())
            {
                rect.MouseDown += Cell_MouseDown;
            }

            ToggleButtonsVisibility(true);
        }

        private void RemoveClickHandlers()
        {
            foreach (Rectangle rect in MazeCanvas.Children.OfType<Rectangle>())
            {
                rect.MouseDown -= Cell_MouseDown;
            }

            ToggleButtonsVisibility(false);
        }

        private void ToggleButtonsVisibility(bool isEditing)
        {
            AllPathsButton.Visibility = isEditing ? Visibility.Hidden : Visibility.Visible;
            ShortestRouteButton.Visibility = isEditing ? Visibility.Hidden : Visibility.Visible;
            NextRouteButton.Visibility = isEditing ? Visibility.Hidden : Visibility.Visible;
            DoneButton.Visibility = isEditing ? Visibility.Visible : Visibility.Collapsed;
        }

        private void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            ClearMaze();
            isEditing = false;
            RouteInfo.Text = "Редактирование завершено.";
            RemoveClickHandlers();
        }

        private void Cell_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is Rectangle rect)
            {
                int x = (int)(Canvas.GetLeft(rect) / cellSize);
                int y = (int)(Canvas.GetTop(rect) / cellSize);

                maze[y, x] = maze[y, x] == 1 ? 0 : 1;
                rect.Fill = maze[y, x] == 1 ? Brushes.Black : Brushes.White;

                Debug.WriteLine($"Ячейка изменена: ({x}, {y}) -> {maze[y, x]}");
            }
        }

        private IEnumerable<Node> GetNeighbors(Node node, int rows, int cols)
        {
            var neighbors = new List<Node>();
            var directions = new Point[]
            {
                new Point(0, -1), new Point(0, 1), new Point(-1, 0), new Point(1, 0)
            };

            foreach (var direction in directions)
            {
                int newX = (int)node.Position.X + (int)direction.X;
                int newY = (int)node.Position.Y + (int)direction.Y;

                if (newX >= 0 && newX < cols && newY >= 0 && newY < rows && maze[newY, newX] != 1)
                {
                    neighbors.Add(new Node(new Point(newX, newY)));
                }
            }

            return neighbors;
        }

        private class Node
        {
            public Point Position { get; }

            public Node(Point position)
            {
                Position = position;
            }
        }
    }
}
