using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp129.SaveSystem.Json;
using ConsoleApp129.SaveSystem.Xml;

namespace ConsoleApp129.SaveSystem
{
    internal static class SaveServiceFactory
    {
        public static ISaveService Create(SaveFormat format)
        {
            switch (format)
            {
                case SaveFormat.Json: return new JsonSaveService();
                case SaveFormat.Xml: return new XmlSaveService();
                default: throw new ArgumentOutOfRangeException(nameof(format), format, "Неизвестный формат.");
            }
        }
    }
}