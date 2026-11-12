using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleApp129.SaveSystem;

namespace ConsoleApp129
{
    public class Map
    {
        Random rand = new Random();
        MapObject[,] map = new MapObject[25, 25];

        public void Map_generation()
        {
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    int A = rand.Next(100);
                    map[i, j] = new Field();

                    if (A > 1 && A < 6) map[i, j] = new Wall();
                    if (A < 1) map[i, j] = new Enemy(i, j);
                    if (A > 5 && A < 10) map[i, j] = new Tree();

                    if (i == map.GetLength(0) / 2 && j == map.GetLength(1) / 2)
                    {
                        map[i, j] = new Hero(i, j);
                    }
                }
            }
        }

        public void Drawing_the_map()
        {
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    Console.Write(map[i, j].Rendering_on_the_map() + " ");
                    Console.ResetColor();
                }
                Console.WriteLine();
            }
        }

        public void MovePersons()
        {
            MapObject[,] newMap = new MapObject[map.GetLength(0), map.GetLength(1)];
            Array.Copy(map, newMap, map.Length);

            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (map[i, j] is Enemy)
                    {
                        int direction = rand.Next(4);
                        int newX = i, newY = j;

                        switch (direction)
                        {
                            case 0: newX = (i - 1 + map.GetLength(0)) % map.GetLength(0); break;
                            case 1: newX = (i + 1) % map.GetLength(0); break;
                            case 2: newY = (j - 1 + map.GetLength(1)) % map.GetLength(1); break;
                            case 3: newY = (j + 1) % map.GetLength(1); break;
                        }

                        if (newMap[newX, newY] is Field)
                        {
                            newMap[newX, newY] = map[i, j];
                            newMap[i, j] = new Field();
                        }
                    }
                }
            }
            Array.Copy(newMap, map, map.Length);
        }

        public void MovePersons(ConsoleKey key)
        {
            MapObject[,] newMap = new MapObject[map.GetLength(0), map.GetLength(1)];
            Array.Copy(map, newMap, map.Length);

            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (map[i, j] is Hero)
                    {
                        int newX = i, newY = j;

                        switch (key)
                        {
                            case ConsoleKey.UpArrow: newX = (i - 1 + map.GetLength(0)) % map.GetLength(0); break;
                            case ConsoleKey.DownArrow: newX = (i + 1) % map.GetLength(0); break;
                            case ConsoleKey.LeftArrow: newY = (j - 1 + map.GetLength(1)) % map.GetLength(1); break;
                            case ConsoleKey.RightArrow: newY = (j + 1) % map.GetLength(1); break;
                        }

                        if (newMap[newX, newY] is Field)
                        {
                            newMap[newX, newY] = map[i, j];
                            newMap[i, j] = new Field();
                        }
                    }
                }
            }
            Array.Copy(newMap, map, map.Length);
        }

        internal IReadOnlyList<Person> GetAllPersons()
        {
            var persons = new List<Person>();
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    Person p = map[i, j] as Person;
                    if (p != null) persons.Add(p);
                }
            }
            return persons;
        }

        public bool HasLiveHero()
        {
            return GetAllPersons().Any(p => p is Hero && p.IsAlive);
        }

        public void CleanupDeadCharacters()
        {
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    Person p = map[i, j] as Person;
                    if (p != null && !p.IsAlive) map[i, j] = new Field();
                }
            }
        }
        public SaveData ToSaveData()
        {
            var data = new SaveData();
            data.SavedAtUtc = DateTime.UtcNow;
            data.Width = map.GetLength(0);
            data.Height = map.GetLength(1);

            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    var cell = new MapCellSaveData { X = i, Y = j };

                    if (map[i, j] is Hero)
                    {
                        cell.Type = "Hero";
                        cell.Person = ToPersonData((Person)map[i, j]);
                    }
                    else if (map[i, j] is Enemy)
                    {
                        cell.Type = "Enemy";
                        cell.Person = ToPersonData((Person)map[i, j]);
                    }
                    else if (map[i, j] is Wall) cell.Type = "Wall";
                    else if (map[i, j] is Tree) cell.Type = "Tree";
                    else cell.Type = "Field";

                    data.Cells.Add(cell);
                }
            }
            return data;
        }

        public void LoadFromSaveData(SaveData data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            map = new MapObject[data.Width, data.Height];

            foreach (var cell in data.Cells)
            {
                switch (cell.Type)
                {
                    case "Wall": map[cell.X, cell.Y] = new Wall(); break;
                    case "Tree": map[cell.X, cell.Y] = new Tree(); break;
                    case "Hero": map[cell.X, cell.Y] = new Hero(cell.X, cell.Y); break;
                    case "Enemy":
                        if (cell.Person != null)
                        {
                            var enemy = new Enemy(cell.X, cell.Y);
                            enemy.Health = cell.Person.Health;
                            enemy.Name = cell.Person.Name;
                            map[cell.X, cell.Y] = enemy;
                        }
                        else map[cell.X, cell.Y] = new Enemy(cell.X, cell.Y);
                        break;
                    default: map[cell.X, cell.Y] = new Field(); break;
                }
            }
        }

        private static PersonSaveData ToPersonData(Person person)
        {
            return new PersonSaveData { Name = person.Name, Health = person.Health, MaxHealth = person.MaxHealth };
        }
    }

}