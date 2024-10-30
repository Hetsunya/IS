using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Maze
{
    public partial class MainWindow : Window
    {
        private MazeSolver maze;
        private MazeController mazeController;
        private List<List<Point>> allPaths = new List<List<Point>>();
        private int currentPathIndex = 0;
        private bool isEditing = false;

        public MainWindow()
        {
            InitializeComponent();

            int[,] mazeArray = {
                { 2, 0, 0, 0, 0, 0, 0, 0, 0, 3 },
                { 0, 1, 1, 0, 1, 1, 1, 1, 0, 1 },
                { 0, 0, 1, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 1, 1, 0, 1, 0, 1, 1, 1, 0 },
                { 0, 0, 0, 0, 0, 1, 1, 0, 1, 0 },
                { 1, 1, 1, 1, 3, 0, 1, 1, 3, 0 }
            };

            int cellSize = 40;
            maze = new MazeSolver(mazeArray, cellSize);
            mazeController = new MazeController(maze);
            maze.Draw(MazeCanvas);
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Text files (*.txt)|*.txt"
            };

            if (fileDialog.ShowDialog() == true)
            {
                MazeFileHandler fileHandler = new MazeFileHandler();
                int[,] loadedMaze = fileHandler.LoadMazeFromTxt(fileDialog.FileName);

                if (loadedMaze != null)
                {
                    maze.SetCells(loadedMaze);
                    maze.Draw(MazeCanvas);
                }
            }
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Text files (*.txt)|*.txt"
            };

            if (fileDialog.ShowDialog() == true)
            {
                MazeFileHandler fileHandler = new MazeFileHandler();
                fileHandler.SaveMazeToTxt(maze.Cells, fileDialog.FileName);
            }
        }

        private void FindAllPaths_Click(object sender, RoutedEventArgs e)
        {
            maze.ClearHighlights(MazeCanvas);
            Point start = maze.FindPoint(2);
            List<Point> ends = maze.FindExits();

            if (start == new Point(-1, -1) || ends == null || ends.Count == 0)
            {
                RouteInfo.Text = "Старт или конец не найдены!";
                return;
            }

            allPaths.Clear();
            allPaths = mazeController.FindAllPaths(start, ends);

            // Очищаем ListBox и добавляем найденные пути в формате [номер] ([длина])
            PathsListBox.Items.Clear();
            for (int i = 0; i < allPaths.Count; i++)
            {
                var path = allPaths[i];
                int length = path.Count; // Длина пути
                PathsListBox.Items.Add($"[{i + 1}] ({length})");
            }

            RouteInfo.Text = allPaths.Count == 0 ? "Пути не найдены!" : $"Найдено путей: {allPaths.Count}";
        }

        // Обработчик выбора пути из списка
        private void PathsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PathsListBox.SelectedItem != null)
            {
                // Очищаем выделения перед подсветкой нового пути
                maze.ClearHighlights(MazeCanvas);

                // Получаем индекс выбранного пути
                int selectedIndex = PathsListBox.SelectedIndex;

                // Подсвечиваем выбранный путь
                if (selectedIndex >= 0 && selectedIndex < allPaths.Count)
                {
                    var selectedPath = allPaths[selectedIndex];
                    HighlightPath(selectedPath, "Yellow");
                }
            }
        }



        private void HighlightPath(List<Point> path, string color)
        {
            foreach (var point in path)
            {
                var child = MazeCanvas.Children
                    .OfType<Rectangle>()
                    .FirstOrDefault(r => Canvas.GetLeft(r) == point.X * maze.CellSize && Canvas.GetTop(r) == point.Y * maze.CellSize);

                if (child != null && (child.Fill != Brushes.Blue && child.Fill != Brushes.Red))
                {
                    child.Fill = color == "Yellow" ? Brushes.Yellow : Brushes.Green;
                }
            }
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

            maze.ClearHighlights(MazeCanvas);
            currentPathIndex = (currentPathIndex + 1) % allPaths.Count;
            HighlightPath(allPaths[currentPathIndex], "Yellow");
            RouteInfo.Text = $"Путь {currentPathIndex + 1} из {allPaths.Count}";
        }

        private void EditMaze_Click(object sender, RoutedEventArgs e)
        {
            isEditing = !isEditing;

            RouteInfo.Text = isEditing ? "Редактирование включено" : "Редактирование завершено";
            if (isEditing)
            {
                EditButton.Content = "Готово";
                AddClickHandlers();
            }
            else
            {
                EditButton.Content = "Изменить конфигурацию лабиринта";
                RemoveClickHandlers();
            }
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
            ImportButton.Visibility = isEditing ? Visibility.Hidden : Visibility.Visible;
            ExportButton.Visibility = isEditing ? Visibility.Hidden : Visibility.Visible;
        }

        private void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            maze.ClearHighlights(MazeCanvas);
            isEditing = false;
            RouteInfo.Text = "Редактирование завершено.";
            RemoveClickHandlers();
        }

        private void Cell_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is Rectangle rect)
            {
                int x = (int)(Canvas.GetLeft(rect) / maze.CellSize);
                int y = (int)(Canvas.GetTop(rect) / maze.CellSize);

                Brush newColor = maze.ToggleCell(x, y);
                rect.Fill = newColor;

                Debug.WriteLine($"Ячейка изменена: ({x}, {y}) -> ({y}, {x})");
            }
        }
    }
}
