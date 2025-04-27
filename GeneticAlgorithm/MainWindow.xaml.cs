using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GeneticAlgorithm
{
    public partial class MainWindow : Window
    {
        private readonly Random random = new();
        private readonly Item[] items = new Item[50];
        private readonly int[] quantities = new int[50];

        public MainWindow()
        {
            InitializeComponent();
            InitializeItems();
            PopulateGrid();
            UpdatePriceRange();
        }

        // Класс для представления товара
        public class Item
        {
            public int Number { get; set; }
            public string Name { get; set; }
            public int Price { get; set; }
            public int MinQuantity { get; set; }
            public int MaxQuantity { get; set; }
            public int SelectedQuantity { get; set; }
            public int TotalCost => Price * SelectedQuantity;
        }

        // Инициализация товаров
        private void InitializeItems()
        {
            string[] names = new string[]
            {
                "Амулет удачи", "Зеркало предсказаний", "Эликсир невидимости", "Песочные часы времени", "Кристалл левитации",
                "Магический посох", "Книга заклинаний", "Светящийся шар", "Кольцо телепортации", "Плащ неуязвимости",
                "Фонарь духов", "Свиток огня", "Талисман силы", "Зелье исцеления", "Кинжал теней",
                "Перо феникса", "Кубок вечности", "Ледяной жезл", "Ковер-самолет", "Часы остановки времени",
                "Меч света", "Щит дракона", "Ключ от всех миров", "Ожерелье звезд", "Сапоги скорости",
                "Корона мудрости", "Браслет стихий", "Зеркало истины", "Флейта призыва", "Алтарь магии",
                "Кольцо иллюзий", "Сфера хаоса", "Посох грома", "Трон теней", "Кубик судьбы",
                "Лампа желаний", "Кольцо времени", "Скрижаль пророчеств", "Пояс гиганта", "Очки ясновидения",
                "Крылья ангела", "Свеча вечности", "Чаша жизни", "Молот грома", "Арфа гармонии",
                "Сумка без дна", "Кинжал молний", "Палочка превращений", "Кольцо защиты", "Книга тайн"
            };

            // Генерация цен
            int[] prices = new int[50];
            for (int i = 0; i < 17; i++) prices[i] = random.Next(1, 10);
            for (int i = 17; i < 34; i++) prices[i] = random.Next(10, 100);
            for (int i = 34; i < 50; i++) prices[i] = random.Next(100, 1000);

            // Генерация мин/макс количеств
            for (int i = 0; i < 50; i++)
            {
                int min = random.Next(1, 5);
                int max = random.Next(min + 2, min + 6);
                items[i] = new Item
                {
                    Number = i + 1,
                    Name = names[i],
                    Price = prices[i],
                    MinQuantity = min,
                    MaxQuantity = max,
                    SelectedQuantity = random.Next(min, max + 1)
                };
                quantities[i] = items[i].SelectedQuantity;
            }
        }

        // Заполнение таблицы
        private void PopulateGrid()
        {
            ItemsGrid.ItemsSource = null;
            ItemsGrid.ItemsSource = items;
        }

        // Обновление диапазона цен
        private void UpdatePriceRange()
        {
            int minPrice = items.Sum(item => item.Price * item.MinQuantity);
            int maxPrice = items.Sum(item => item.Price * item.MaxQuantity);
            PriceRangeLabel.Text = $"Диапазон цен: от {minPrice} до {maxPrice}";
        }

        // Валидация ввода чисел
        private void NumberValidation(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !char.IsDigit(e.Text, 0);
        }

        // Обработчик кнопки "Запустить"
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TargetSum.Text) || string.IsNullOrEmpty(Iterations.Text))
            {
                MessageBox.Show("Введите сумму и количество итераций!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int targetSum = int.Parse(TargetSum.Text);
            int iterations = int.Parse(Iterations.Text);

            if (targetSum == 0)
            {
                MessageBox.Show("Сумма не может быть нулевой!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Генетический алгоритм
            (int[] bestSolution, int usedIterations, bool isPerfect) = RunGeneticAlgorithm(targetSum, iterations);

            // Обновление выбранных количеств
            for (int i = 0; i < 50; i++)
            {
                items[i].SelectedQuantity = bestSolution[i];
            }

            // Обновление таблицы
            PopulateGrid();

            // Вывод результатов
            int finalSum = items.Sum(item => item.TotalCost);
            int difference = Math.Abs(finalSum - targetSum);
            ResultSumLabel.Text = $"Итоговая сумма: {finalSum}";
            DifferenceLabel.Text = $"Разница с желаемой суммой: {difference}";
            IterationResultLabel.Text = isPerfect
                ? $"Найдено идеальное решение на итерации {usedIterations}"
                : $"Идеальное решение не найдено, использовано итераций: {usedIterations}";
        }

        // Генетический алгоритм
        private (int[] bestSolution, int usedIterations, bool isPerfect) RunGeneticAlgorithm(int targetSum, int maxIterations)
        {
            // Начальная популяция (одна особь)
            int[] original = new int[50];
            for (int i = 0; i < 50; i++)
            {
                original[i] = random.Next(items[i].MinQuantity, items[i].MaxQuantity + 1);
            }

            int[] bestSolution = (int[])original.Clone();
            int bestFitness = CalculateFitness(original, targetSum);
            int usedIterations = 0;
            bool isPerfect = false;

            for (int iter = 0; iter < maxIterations; iter++)
            {
                usedIterations = iter + 1;

                // Мутация: создаём две особи с одной мутацией
                int[] mutated1 = (int[])original.Clone();
                int[] mutated2 = (int[])original.Clone();

                // Мутация в первой половине для первой особи
                int index1 = random.Next(0, 25);
                mutated1[index1] = random.Next(items[index1].MinQuantity, items[index1].MaxQuantity + 1);

                // Мутация во второй половине для второй особи
                int index2 = random.Next(25, 50);
                mutated2[index2] = random.Next(items[index2].MinQuantity, items[index2].MaxQuantity + 1);

                // Скрещивание двух мутировавших особей
                int[] cross1 = new int[50];
                int[] cross2 = new int[50];
                for (int i = 0; i < 25; i++)
                {
                    cross1[i] = mutated1[i];
                    cross2[i] = mutated2[i];
                }
                for (int i = 25; i < 50; i++)
                {
                    cross1[i] = mutated2[i];
                    cross2[i] = mutated1[i];
                }

                // Оценка всех популяций
                var populations = new[] { original, mutated1, mutated2, cross1, cross2 };
                int bestPopIndex = 0;
                int minFitness = CalculateFitness(original, targetSum);

                for (int i = 1; i < populations.Length; i++)
                {
                    int fitness = CalculateFitness(populations[i], targetSum);
                    if (fitness < minFitness)
                    {
                        minFitness = fitness;
                        bestPopIndex = i;
                    }
                }

                // Обновление лучшего решения
                if (minFitness < bestFitness)
                {
                    bestFitness = minFitness;
                    bestSolution = (int[])populations[bestPopIndex].Clone();
                }

                // Проверка на точное совпадение
                if (minFitness == 0)
                {
                    isPerfect = true;
                    break;
                }

                // Новая оригинальная популяция
                original = (int[])populations[bestPopIndex].Clone();
            }

            return (bestSolution, usedIterations, isPerfect);
        }

        // Вычисление приспособленности
        private int CalculateFitness(int[] solution, int targetSum)
        {
            int total = 0;
            for (int i = 0; i < 50; i++)
            {
                total += solution[i] * items[i].Price;
            }
            return Math.Abs(total - targetSum);
        }
    }
}