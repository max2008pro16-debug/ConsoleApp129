using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ConsoleApp129.SaveSystem.Json
{
    internal sealed class JsonSaveService : ISaveService
    {
        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions { WriteIndented = true };

        public string FileExtension { get { return ".json"; } }

        public void Save(SaveData data, string filePath)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            var json = JsonSerializer.Serialize(data, Options);
            File.WriteAllText(filePath, json);
        }

        public SaveData Load(string filePath)
        {
            if (!File.Exists(filePath)) throw new FileNotFoundException("Файл не найден.", filePath);
            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<SaveData>(json, Options);
        }
    }
}