using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp129
{
    /// Базовый класс для всех персонажей (герой, враги).
    /// Наследует MapObject, добавляя координаты и здоровье.

    public class Person : MapObject
    {
        public int X { get; set; }
        public int Y { get; set; }

        /// Здоровье персонажа 
        public int Health { get; set; } = 100;

        public Person(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override (char Symbol, ConsoleColor Color) Render()
            => ('☺', ConsoleColor.Red);
    }

    // Класс Героя (наследуется от Person)
    public class Hero : Person
    {
        /// Очки героя 
        public int Score { get; set; } = 0;

        public Hero(int x, int y) : base(x, y) { }

        public override (char Symbol, ConsoleColor Color) Render()
            => ('☻', ConsoleColor.Yellow);
    }

    // Класс Врага (наследуется от Person)
    public class Enemy : Person
    {
        public Enemy(int x, int y) : base(x, y) { }

        public override (char Symbol, ConsoleColor Color) Render()
            => ('☺', ConsoleColor.Red);
    }
}