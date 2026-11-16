using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp129
{
    /// Абстрактный базовый класс для всех объектов на карте.
    internal abstract class MapObject
    {
        /// Возвращает символ для отображения объекта в консоли.
        public abstract char RenderOnMap();
    }

    /// Класс, описывающий препятствие "Стена".
    internal class Wall : MapObject
    {
        public override char RenderOnMap()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            return '+';
        }
    }

    /// Класс, описывающий свободную клетку "Поле".
    internal class Field : MapObject
    {
        public override char RenderOnMap()
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            return '.';
        }
    }

    /// Класс, описывающий препятствие "Дерево".
    internal class Tree : MapObject
    {
        public override char RenderOnMap()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            return 'T';
        }
    }

    /// Череп (лут с врагов).
    internal class Skull : MapObject
    {
        public override char RenderOnMap()
        {
            Console.ForegroundColor = ConsoleColor.White;
            return '☠';
        }
    }

    /// Портал на следующий уровень.
    internal class Portal : MapObject
    {
        public override char RenderOnMap()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            return 'O';
        }
    }
}