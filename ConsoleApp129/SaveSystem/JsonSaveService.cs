using ConsoleApp129.Core;
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
            if (data == null) throw new GameException("Нет данных для сохранения!", "SAVE_NULL");

            try
            {
                var serializer = new DataContractJsonSerializer(typeof(SaveData));
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    serializer.WriteObject(stream, data);
                }
            }
            catch (Exception ex)
            {
                throw new GameException("Не удалось записать файл сохранения на диск.", "SAVE_IO_ERROR", ex);
            }
        }

        public SaveData Load(string filePath)
        {
            if (!File.Exists(filePath))
                throw new GameException("Файл сохранения не найден!", "SAVE_FILE_NOT_FOUND");

            try
            {
                var serializer = new DataContractJsonSerializer(typeof(SaveData));
                using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    return (SaveData)serializer.ReadObject(stream);
                }
            }
            catch (Exception ex)
            {
                throw new GameException("Файл сохранения поврежден или имеет неверный формат.", "SAVE_CORRUPTED", ex);
            }
        }
    }
}