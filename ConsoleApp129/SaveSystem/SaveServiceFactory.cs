using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp129.SaveSystem;

namespace ConsoleApp129.SaveSystem
{
    internal static class SaveServiceFactory
    {
        public static ISaveService Create(SaveFormat format)
        {
            return new JsonSaveService();
        }
    }
}