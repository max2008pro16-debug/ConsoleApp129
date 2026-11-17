using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp129.SaveSystem
{
    internal interface ISaveService
    {
        string FileExtension { get; }
        void Save(SaveData data, string filePath);
        SaveData Load(string filePath);
    }
}