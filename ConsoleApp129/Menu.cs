using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp129
{
    /// Базовый класс для всех меню игры.
    /// Реализует общий механизм навигации стрелками + Enter.
    /// Наследники определяют набор пунктов и что делать при выборе.
    public abstract class BaseMenu
    {
        protected readonly List<string> Items = new List<string>();
        protected int SelectedIndex = 0;
        protected string Title = "";

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        /// Запускает цикл отображения и выбора.
        /// Возвращает индекс выбранного пункта.
        public virtual int Run()
        {
            while (true)
            {
                Console.Clear();
                if (!string.IsNullOrEmpty(Title))
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(Title);
                    Console.ResetColor();
                    Console.WriteLine(new string('=', Title.Length));
                }

                for (int i = 0; i < Items.Count; i++)
                {
                    if (i == SelectedIndex)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGray;
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    Console.WriteLine("  " + Items[i]);
                    Console.ResetColor();
                }

                Console.WriteLine("\n↑↓ — навигация, Enter — выбор, Esc — назад");
                var key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        SelectedIndex = (SelectedIndex - 1 + Items.Count) % Items.Count;
                        break;
                    case ConsoleKey.DownArrow:
                        SelectedIndex = (SelectedIndex + 1) % Items.Count;
                        break;
                    case ConsoleKey.Enter:
                        return SelectedIndex;
                    case ConsoleKey.Escape:
                        return -1;
                }
            }
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
    public class MainMenu : BaseMenu
    {
        public enum Action { NewGame = 0, Load = 1, Exit = 2 }

        public MainMenu()
        {
            Title = "ГЛАВНОЕ МЕНЮ";
            Items.Add("Новая игра");
            Items.Add("Загрузить игру");
            Items.Add("Выход");
        }

        public Action GetChoice() => (Action)Run();
    }
    public class GameMenu : BaseMenu
    {
        public enum Action { Continue = 0, Save = 1, Load = 2, Character = 3, ToMain = 4 }

        public GameMenu()
        {
            Title = "ИГРОВОЕ МЕНЮ";
            Items.Add("Продолжить");
            Items.Add("Сохранить");
            Items.Add("Загрузить");
            Items.Add("Меню персонажа");
            Items.Add("В главное меню");
        }

        public Action GetChoice()
        {
            int choice = Run();
            if (choice == -1) return Action.Continue; // Esc в игре = продолжить
            return (Action)choice;
        }
    }
}
