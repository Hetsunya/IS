using System;
using System.IO;
using Newtonsoft.Json;

namespace Maze
{
    public class MazeFileHandler
    {
        public int[,] LoadMaze(string filePath)
        {
            try
            {
                string json = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<int[,]>(json);
            }
            catch (Exception ex)
            {
                // Обработка ошибок
                Console.WriteLine($"Ошибка при загрузке лабиринта: {ex.Message}");
                return null;
            }
        }

        public void SaveMaze(int[,] maze, string filePath)
        {
            try
            {
                string json = JsonConvert.SerializeObject(maze);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                // Обработка ошибок
                Console.WriteLine($"Ошибка при сохранении лабиринта: {ex.Message}");
            }
        }
    }
}
