using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp129
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // нужен, чтобы символы корректно отображались в консоли
            Console.OutputEncoding = Encoding.UTF8;
            Console.CursorVisible = false;

            Map map = new Map();
            map.Map_generation();

            while (true)
            {
                Console.Clear();
                map.Drawing_the_map();

                Console.WriteLine();
                Console.WriteLine("Стрелки - движение, Esc - выход");

                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                if (keyInfo.Key == ConsoleKey.Escape)
                    break;

                // 1) двигаем героя согласно нажатой клавише
                map.MovePersons(keyInfo.Key);

                // 2) двигаем врагов случайным образом
                map.MovePersons();
            }

            Console.CursorVisible = true;
        }
    }
}