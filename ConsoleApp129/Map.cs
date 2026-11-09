using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp129
{
    // Класс, отвечающий за генерацию, отрисовку и логику карты
    internal class Map
    {
        // Генератор случайных чисел для расстановки объектов и движения врагов
        Random rand = new Random();

        // Двумерный массив 25x25, хранящий все объекты на карте
        MapObject[,] map = new MapObject[25, 25];

        // Метод первичной генерации карты при запуске программы
        public void Map_generation()
        {
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    int A = rand.Next(100); // Генерируем число от 0 до 99
                    map[i, j] = new Field(); // По умолчанию клетка пустая

                    // Если выпало 2-5, ставим стену
                    if (A > 1 && A < 6) map[i, j] = new Wall();
                    // Если выпало 0, ставим врага
                    if (A < 1) map[i, j] = new Enemy(i, j);
                    // Если выпало 6-9, ставим дерево
                    if (A > 5 && A < 10) map[i, j] = new Tree();

                    // Принудительно ставим героя ровно по центру карты (12, 12)
                    if (i == map.GetLength(0) / 2 && j == map.GetLength(1) / 2)
                    {
                        map[i, j] = new Hero(i, j);
                    }
                }
            }
        }

        // Метод отрисовки всей карты в консоли
        public void Drawing_the_map()
        {
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    // Получаем символ объекта и выводим его с пробелом
                    Console.Write(map[i, j].Rendering_on_the_map() + " ");
                    Console.ResetColor(); // Сбрасываем цвет для следующего символа
                }
                Console.WriteLine(); // Переход на новую строку после каждого ряда
            }
        }

        // Метод для случайного перемещения врагов по карте
        public void MovePersons()
        {
            // Создаем временную карту, чтобы избежать багов при одновременном движении
            MapObject[,] newMap = new MapObject[map.GetLength(0), map.GetLength(1)];
            Array.Copy(map, newMap, map.Length);

            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (map[i, j] is Enemy) // Ищем врагов на карте
                    {
                        int direction = rand.Next(4); // Случайное направление (0-3)
                        int newX = i, newY = j;

                        // Вычисляем новые координаты в зависимости от направления
                        switch (direction)
                        {
                            case 0: newX = (i - 1 + map.GetLength(0)) % map.GetLength(0); break; // Вверх
                            case 1: newX = (i + 1) % map.GetLength(0); break; // Вниз
                            case 2: newY = (j - 1 + map.GetLength(1)) % map.GetLength(1); break; // Влево
                            case 3: newY = (j + 1) % map.GetLength(1); break; // Вправо
                        }

                        // Если в новой клетке пусто (Field), враг туда перемещается
                        if (newMap[newX, newY] is Field)
                        {
                            newMap[newX, newY] = map[i, j];
                            newMap[i, j] = new Field();
                        }
                    }
                }
            }
            Array.Copy(newMap, map, map.Length); // Применяем изменения
        }

        // Перегруженный метод для перемещения Героя (управляется игроком)
        public void MovePersons(ConsoleKey key)
        {
            MapObject[,] newMap = new MapObject[map.GetLength(0), map.GetLength(1)];
            Array.Copy(map, newMap, map.Length);

            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (map[i, j] is Hero) // Ищем Героя на карте
                    {
                        int newX = i, newY = j;

                        // Меняем координаты в зависимости от нажатой стрелки
                        switch (key)
                        {
                            case ConsoleKey.UpArrow: newX = (i - 1 + map.GetLength(0)) % map.GetLength(0); break;
                            case ConsoleKey.DownArrow: newX = (i + 1) % map.GetLength(0); break;
                            case ConsoleKey.LeftArrow: newY = (j - 1 + map.GetLength(1)) % map.GetLength(1); break;
                            case ConsoleKey.RightArrow: newY = (j + 1) % map.GetLength(1); break;
                        }

                        // Если клетка пустая (Field), перемещаем героя
                        if (newMap[newX, newY] is Field)
                        {
                            newMap[newX, newY] = map[i, j];
                            newMap[i, j] = new Field();
                        }
                    }
                }
            }
            Array.Copy(newMap, map, map.Length);
        }
    }
}
