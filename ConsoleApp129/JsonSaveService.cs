using System;
using System.IO;
using System.Runtime.Serialization.Json;

namespace ConsoleApp129.SaveSystem
{
    internal sealed class JsonSaveService : ISaveService
    {
        public string FileExtension { get { return ".json"; } }

        public void Save(SaveData data, string filePath)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            var serializer = new DataContractJsonSerializer(typeof(SaveData));
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                serializer.WriteObject(stream, data);
            }
        }

        public SaveData Load(string filePath)
        {
            if (!File.Exists(filePath)) throw new FileNotFoundException("Файл не найден.", filePath);

            var serializer = new DataContractJsonSerializer(typeof(SaveData));
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                return (SaveData)serializer.ReadObject(stream);
            }
        }
    }
}