using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ConsoleApp129.Menus
{
    internal sealed class MenuItem
    {
        public string Title { get; private set; }
        public Action Action { get; private set; }

        public MenuItem(string title, Action action)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Заголовок не может быть пустым.", nameof(title));
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            Title = title;
            Action = action;
        }
    }
}