using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ConsoleApp129.SaveSystem.Xml
{
    internal sealed class XmlSaveService : ISaveService
    {
        private static readonly XmlSerializer Serializer = new XmlSerializer(typeof(SaveData));

        public string FileExtension { get { return ".xml"; } }

        public void Save(SaveData data, string filePath)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            using (var stream = File.Create(filePath))
            {
                Serializer.Serialize(stream, data);
            }
        }

        public SaveData Load(string filePath)
        {
            if (!File.Exists(filePath)) throw new FileNotFoundException("Файл не найден.", filePath);
            using (var stream = File.OpenRead(filePath))
            {
                return Serializer.Deserialize(stream) as SaveData;
            }
        }
    }
}