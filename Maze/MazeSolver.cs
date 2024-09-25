using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Maze
{
    public class MazeSolver
    {
        public int[,] Cells { get; private set; }
        public int CellSize { get; private set; }

        public MazeSolver(int[,] cells, int cellSize)
        {
            Cells = cells;
            CellSize = cellSize;
        }

        public void Draw(Canvas canvas)
        {
            canvas.Children.Clear();

            for (int i = 0; i < Cells.GetLength(0); i++)
            {
                for (int j = 0; j < Cells.GetLength(1); j++)
                {
                    Rectangle rect = new Rectangle
                    {
                        Width = CellSize,
                        Height = CellSize,
                        Stroke = Brushes.Black,
                        Fill = Cells[i, j] switch
                        {
                            0 => Brushes.White,
                            1 => Brushes.Black,
                            2 => Brushes.Blue,
                            _ => Brushes.Red
                        }
                    };
                    Canvas.SetLeft(rect, j * CellSize);
                    Canvas.SetTop(rect, i * CellSize);
                    canvas.Children.Add(rect);
                }
            }
        }

        public Brush ToggleCell(int x, int y)
        {
            // Меняем состояние ячейки
            Cells[y, x] = Cells[y, x] == 1 ? 0 : 1;

            // Возвращаем цвет в зависимости от нового состояния ячейки
            return Cells[y, x] == 1 ? Brushes.Black : Brushes.White;
        }

        public Point FindPoint(int value)
        {
            for (int i = 0; i < Cells.GetLength(0); i++)
            {
                for (int j = 0; j < Cells.GetLength(1); j++)
                {
                    if (Cells[i, j] == value)
                        return new Point(j, i);
                }
            }
            return new Point(-1, -1);
        }

        public void ClearHighlights(Canvas canvas)
        {
            foreach (var child in canvas.Children.OfType<Rectangle>())
            {
                if (child.Fill == Brushes.Yellow || child.Fill == Brushes.Green)
                {
                    child.Fill = Brushes.White;
                }
            }
        }
    }
}

