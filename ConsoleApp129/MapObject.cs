using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp129
{
    // Абстрактный базовый класс для всех объектов на карте.
    // Задает общий интерфейс для отрисовки.
    internal abstract class MapObject
    {
        // Абстрактный метод, который обязан реализовать каждый наследник.
        // Возвращает символ для отображения объекта в консоли.
        public abstract char Rendering_on_the_map();
    }

    // Класс, описывающий препятствие "Стена"
    internal class Wall : MapObject
    {
        // Метод отрисовки стены. Красит символ в голубой цвет.
        public override char Rendering_on_the_map()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            return '+';
        }
    }

    // Класс, описывающий свободную клетку "Поле"
    internal class Field : MapObject
    {
        // Метод отрисовки поля. Красит символ в темно-зеленый цвет.
        public override char Rendering_on_the_map()
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            return '.';
        }
    }

    // Класс, описывающий препятствие "Дерево"
    internal class Tree : MapObject
    {
        // Метод отрисовки дерева. Красит символ в зеленый цвет.
        public override char Rendering_on_the_map()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            return 'T';
        }
    }
}
