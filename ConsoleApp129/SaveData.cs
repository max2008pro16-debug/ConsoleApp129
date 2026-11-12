using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp129.SaveSystem
{
    internal enum SaveFormat { Json, Xml }

    public sealed class PersonSaveData
    {
        public string Name { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
    }

    public sealed class MapCellSaveData
    {
        public int X { get; set; }
        public int Y { get; set; }
        public string Type { get; set; }
        public PersonSaveData Person { get; set; }
    }

    public sealed class SaveData
    {
        public DateTime SavedAtUtc { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public List<MapCellSaveData> Cells { get; set; }

        public SaveData()
        {
            SavedAtUtc = DateTime.UtcNow;
            Cells = new List<MapCellSaveData>();
        }
    }
}