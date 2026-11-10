using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp129
{
    // Абстрактный базовый класс для всех объектов на карте.
    // Задает общий интерфейс для отрисовки.
    /// Реализует принцип полиморфизма: каждый наследник определяет
    /// своё визуальное представление.
    public abstract class MapObject
    {
        /// Возвращает символ и цвет для отрисовки объекта в консоли.
        /// Возврат кортежа вместо установки цвета внутри метода позволяет
        /// отделить логику объекта от побочных эффектов рендеринга
        public abstract (char Symbol, ConsoleColor Color) Render();
    }

    // Класс, описывающий препятствие "Стена"
    public class Wall : MapObject
    {
        public override (char Symbol, ConsoleColor Color) Render()
            => ('+', ConsoleColor.Cyan);
    }

    // Класс, описывающий свободную клетку "Поле"
    public class Field : MapObject
    {
        public override (char Symbol, ConsoleColor Color) Render()
            => ('.', ConsoleColor.DarkGreen);
    }

    // Класс, описывающий препятствие "Дерево"
    public class Tree : MapObject
    {
        public override (char Symbol, ConsoleColor Color) Render()
            => ('T', ConsoleColor.Green);
    }
    /// «Лес» — новый тип объекта, введённый в ПР №2.
    /// Отличается от одиночного дерева символом и цветом,
    /// визуально обозначает густой лес.
    public class Menu : MapObject
    {
        protected Menu()
        {
        }

        public override (char Symbol, ConsoleColor Color) Render()
            => ('♣', ConsoleColor.DarkGreen);
    }
    /// «Лес» — новый тип объекта
    public class Forest : MapObject
    {
        public override (char Symbol, ConsoleColor Color) Render()
        {
            return ('♣', ConsoleColor.DarkGreen);
        }
    }
    /// «Гора» — непроходимое препятствие
    public class Mountain : MapObject
    {
        public override (char Symbol, ConsoleColor Color) Render()
        {
            return ('▲', ConsoleColor.Gray);
        }
    }
}
