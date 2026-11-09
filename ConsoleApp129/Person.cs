using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp129
{
    // Базовый класс для всех персонажей на карте (наследует MapObject)
    internal class Person : MapObject
    {
        // Приватные поля для хранения координат персонажа
        int pointX;
        int pointY;

        // Конструктор, принимающий начальные координаты X и Y
        public Person(int X, int Y)
        {
            pointX = X;
            pointY = Y;
        }

        // Метод отрисовки обычного персонажа (по умолчанию - врага).
        // Возвращает символ смайлика.
        public override char Rendering_on_the_map()
        {
            return '☺';
        }
    }

    // Класс Героя (наследуется от Person)
    internal class Hero : Person
    {
        // Конструктор героя, вызывает конструктор базового класса Person
        public Hero(int X, int Y) : base(X, Y)
        {
        }

        // Переопределенный метод отрисовки героя.
        // Задает желтый цвет и уникальный символ '☻'.
        public override char Rendering_on_the_map()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            return '☻';
        }
    }

    // Класс Врага (наследуется от Person)
    internal class Enemy : Person
    {
        // Конструктор врага
        public Enemy(int X, int Y) : base(X, Y)
        {
        }

        // Переопределенный метод отрисовки врага.
        // Красит стандартный смайлик в красный цвет.
        public override char Rendering_on_the_map()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            return base.Rendering_on_the_map();
        }
    }
}