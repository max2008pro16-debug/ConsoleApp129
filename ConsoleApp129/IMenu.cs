using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ConsoleApp129.Menus
{
    internal interface IMenu
{
    string Title { get; }
    void Show();
}
}