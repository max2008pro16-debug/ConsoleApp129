using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp129.Core
{
    internal static class ConsoleHelper
    {
        public static void Pause()
        {
            Console.WriteLine();
            Console.Write("Нажмите Enter для продолжения...");
            Console.ReadLine();
        }
    }
}