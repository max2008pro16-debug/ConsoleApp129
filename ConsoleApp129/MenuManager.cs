using ConsoleApp129.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp129.Core;

namespace ConsoleApp129.Menus
{
    internal sealed class MenuManager
    {
        private readonly string _title;
        private readonly List<MenuItem> _items;

        public MenuManager(string title, IEnumerable<MenuItem> items)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Заголовок не может быть пустым.", nameof(title));
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            _title = title;
            _items = new List<MenuItem>(items);

            if (_items.Count == 0)
                throw new ArgumentException("Меню должно содержать хотя бы один пункт.", nameof(items));
        }

        public void Run()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine(_title);
                Console.WriteLine(new string('-', _title.Length));

                for (var i = 0; i < _items.Count; i++)
                {
                    Console.WriteLine("{0}. {1}", i + 1, _items[i].Title);
                }

                Console.WriteLine("0. Назад/Выход");
                Console.Write("Выберите пункт: ");

                var input = Console.ReadLine();
                int choice;

                if (!int.TryParse(input, out choice))
                {
                    Console.WriteLine("Некорректный ввод. Введите число.");
                    ConsoleHelper.Pause();
                    continue;
                }

                if (choice == 0) return;

                if (choice < 1 || choice > _items.Count)
                {
                    Console.WriteLine("Такого пункта меню нет.");
                    ConsoleHelper.Pause();
                    continue;
                }

                Console.Clear();

                try
                {
                    _items[choice - 1].Action();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка: " + ex.Message);
                }

                ConsoleHelper.Pause();
            }
        }
    }
}