using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ConsoleApp129.SaveSystem
{
    internal enum SaveFormat { Json }

    [DataContract]
    public sealed class PersonSaveData
    {
        [DataMember] public string Name { get; set; }
        [DataMember] public int Health { get; set; }
        [DataMember] public int MaxHealth { get; set; }
    }

    [DataContract]
    public sealed class MapCellSaveData
    {
        [DataMember] public int X { get; set; }
        [DataMember] public int Y { get; set; }
        [DataMember] public string Type { get; set; }
        [DataMember] public PersonSaveData Person { get; set; }
    }

    [DataContract]
    public sealed class SaveData
    {
        [DataMember] public string SavedAtUtc { get; set; }
        [DataMember] public int Width { get; set; }
        [DataMember] public int Height { get; set; }
        [DataMember] public List<MapCellSaveData> Cells { get; set; }

        public SaveData()
        {
            SavedAtUtc = DateTime.UtcNow.ToString("o");
            Cells = new List<MapCellSaveData>();
        }
    }
}