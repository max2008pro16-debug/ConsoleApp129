using ConsoleApp129;
using System;
using System.IO;
using System.Text.Json;
using static ConsoleApp129.SaveData;

namespace ConsoleApp129
{
    /// Менеджер сохранений. Отвечает за сериализацию/десериализацию
    /// карты в JSON-файл.
    public static class SaveManager
    {
        private const string SaveFile = "savegame.json";

        /// Сериализует карту в JSON-файл.
        public static void Save(Map map)
        {
            var data = new SaveData
            {
                Width = map.Width,
                Height = map.Height
            };

            for (int i = 0; i < map.Width; i++)
            {
                for (int j = 0; j < map.Height; j++)
                {
                    var obj = map.Cells[i, j];
                    if (obj is Field) continue; // поля не сохраняем

                    // Классическая проверка типов вместо switch-выражения
                    string type = null;
                    if (obj is Wall) type = "Wall";
                    else if (obj is Tree) type = "Tree";
                    else if (obj is Forest) type = "Forest";
                    else if (obj is Mountain) type = "Mountain";
                    else if (obj is Enemy) type = "Enemy";
                    else if (obj is Hero) type = "Hero";

                    if (type == null) continue;

                    data.Objects.Add(new ObjectDto { Type = type, X = i, Y = j });

                    // Проверка с паттерном 
                    if (obj is Hero h)
                    {
                        data.HeroX = i;
                        data.HeroY = j;
                        data.HeroHealth = h.Health;
                        data.HeroScore = h.Score;
                    }
                }
            }

            var options = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(SaveFile, JsonSerializer.Serialize(data, options));
        }

        /// Загружает карту из JSON-файла.
        public static Map Load()
        {
            if (!File.Exists(SaveFile))
                throw new FileNotFoundException("Сохранение не найдено");

            var data = JsonSerializer.Deserialize<SaveData>(File.ReadAllText(SaveFile));
            var map = new Map();
            map.Generate(data.Width, data.Height);

            // Очищаем карту
            for (int i = 0; i < map.Width; i++)
            {
                for (int j = 0; j < map.Height; j++)
                {
                    map.Cells[i, j] = new Field();
                }
            }

            // Восстанавливаем объекты
            foreach (var obj in data.Objects)
            {
                MapObject loadedObject = new Field();

                if (obj.Type == "Wall") loadedObject = new Wall();
                else if (obj.Type == "Tree") loadedObject = new Tree();
                else if (obj.Type == "Forest") loadedObject = new Forest();
                else if (obj.Type == "Mountain") loadedObject = new Mountain();
                else if (obj.Type == "Enemy") loadedObject = new Enemy(obj.X, obj.Y);
                else if (obj.Type == "Hero")
                {
                    loadedObject = new Hero(obj.X, obj.Y)
                    {
                        Health = data.HeroHealth,
                        Score = data.HeroScore
                    };
                }

                map.Cells[obj.X, obj.Y] = loadedObject;
            }

            return map;
        }
    }
}
