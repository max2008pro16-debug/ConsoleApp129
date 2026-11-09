using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp129
{
    internal class Program
    {
        // Точка входа в программу (метод Main)
        static void Main(string[] args)
        {
            // Устанавливаем кодировку UTF-8, чтобы смайлики отображались корректно
            Console.OutputEncoding = Encoding.UTF8;
            // Скрываем мигающий курсор в консоли для эстетики
            Console.CursorVisible = false;

            Map map = new Map(); // Создаем экземпляр карты
            map.Map_generation(); // Генерируем начальное расположение объектов

            // Бесконечный игровой цикл
            while (true)
            {
                Console.Clear(); // Очищаем консоль перед каждым кадром
                map.Drawing_the_map(); // Отрисовываем актуальное состояние карты

                Console.WriteLine("\nСтрелки - движение, Esc - выход");
                ConsoleKeyInfo keyInfo = Console.ReadKey(true); // Ждем нажатия клавиши

                // Если нажата клавиша Esc, выходим из цикла и завершаем программу
                if (keyInfo.Key == ConsoleKey.Escape) break;

                // Сначала ходит игрок
                map.MovePersons(keyInfo.Key);
                // Затем ходят враги
                map.MovePersons();
            }

            Console.CursorVisible = true; // Возвращаем курсор обратно при выходе
        }
    }
}