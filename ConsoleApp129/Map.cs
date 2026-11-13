using ConsoleApp129.Core;
using ConsoleApp129.SaveSystem;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp129
{
    internal class Map
    {
        Random rand = new Random();
        MapObject[,] map = new MapObject[25, 25];

        public int ParalysisTurns { get; set; } = 0;

        public void Map_generation()
        {
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    map[i, j] = new Field();
                    int A = rand.Next(100);

                    if (A > 1 && A < 6) map[i, j] = new Wall();
                    if (A < 1) map[i, j] = new Enemy(i, j);
                    if (A > 5 && A < 10) map[i, j] = new Tree();

                    if (i == 12 && j == 12) map[i, j] = new Hero(i, j);
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

        // ХОД ГЕРОЯ
        public void MovePersons(ConsoleKey key)
        {
            MapObject[,] newMap = new MapObject[map.GetLength(0), map.GetLength(1)];
            Array.Copy(map, newMap, map.Length);

            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (map[i, j] is Hero hero)
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
                        else if (newMap[newX, newY] is Wall)
                        {
                            hero.InventoryWalls++;
                            newMap[newX, newY] = map[i, j];
                            newMap[i, j] = new Field();
                        }
                        else if (newMap[newX, newY] is Tree)
                        {
                            hero.InventoryTrees++;
                            newMap[newX, newY] = map[i, j];
                            newMap[i, j] = new Field();
                        }
                        // АТАКА ГЕРОЯ
                        else if (newMap[newX, newY] is Enemy enemy)
                        {
                            enemy.TakeDamage(20); // Герой бьет на 20

                            // ВРАГ ДАЕТ СДАЧИ ТОЛЬКО ЕСЛИ ОН НЕ ЗАМОРОЖЕН
                            if (ParalysisTurns == 0)
                            {
                                hero.TakeDamage(10);
                                Console.WriteLine("Вы ударили врага, он дал сдачи! (Нажмите любую клавишу)");
                            }
                            else
                            {
                                Console.WriteLine("Вы избили замороженного врага! (Нажмите любую клавишу)");
                            }
                            Console.ReadKey(true); // Ждем нажатия, чтобы прочитать текст

                            if (!enemy.IsAlive)
                            {
                                newMap[newX, newY] = map[i, j];
                                newMap[i, j] = new Field();
                            }
                            if (!hero.IsAlive)
                            {
                                newMap[i, j] = new Field();
                            }
                        }
                    }
                }
            }
            Array.Copy(newMap, map, map.Length);
        }

        public void PlaceObject(char type)
        {
            Hero hero = GetAllPersons().FirstOrDefault(p => p is Hero) as Hero;
            if (hero == null) return;

            int hx = -1, hy = -1;
            for (int i = 0; i < map.GetLength(0); i++)
                for (int j = 0; j < map.GetLength(1); j++)
                    if (map[i, j] == hero) { hx = i; hy = j; }

            int tx = (hx + 1) % map.GetLength(0);
            int ty = hy;

            if (type == 'W' && hero.InventoryWalls > 0 && map[tx, ty] is Field)
            {
                map[tx, ty] = new Wall();
                hero.InventoryWalls--;
                CheckPatterns();
            }
            else if (type == 'T' && hero.InventoryTrees > 0 && map[tx, ty] is Field)
            {
                map[tx, ty] = new Tree();
                hero.InventoryTrees--;
                CheckPatterns();
            }
        }

        private void CheckPatterns()
        {
            for (int i = 1; i < map.GetLength(0) - 1; i++)
            {
                for (int j = 1; j < map.GetLength(1) - 1; j++)
                {
                    if (map[i, j] is Wall && map[i - 1, j] is Wall && map[i + 1, j] is Wall &&
                        map[i, j - 1] is Wall && map[i, j + 1] is Wall)
                    {
                        map[i, j] = new Field(); map[i - 1, j] = new Field(); map[i + 1, j] = new Field();
                        map[i, j - 1] = new Field(); map[i, j + 1] = new Field();

                        if (map[i - 1, j - 1] is Field) map[i - 1, j - 1] = new Ally(i - 1, j - 1);
                        if (map[i + 1, j + 1] is Field) map[i + 1, j + 1] = new Ally(i + 1, j + 1);
                        return;
                    }

                    if (map[i, j] is Tree &&
                        map[i - 1, j - 1] is Tree && map[i - 1, j] is Tree && map[i - 1, j + 1] is Tree &&
                        map[i + 1, j] is Tree)
                    {
                        map[i, j] = new Field();
                        map[i - 1, j - 1] = new Field(); map[i - 1, j] = new Field(); map[i - 1, j + 1] = new Field();
                        map[i + 1, j] = new Field();

                        ParalysisTurns = 30;
                        return;
                    }
                }
            }
        }

        public void MovePersons()
        {
            // Если заморозка - враги пропускают ход
            if (ParalysisTurns > 0)
            {
                ParalysisTurns--;
                MoveAllies();
                return;
            }

            MoveAllies();

            MapObject[,] newMap = new MapObject[map.GetLength(0), map.GetLength(1)];
            Array.Copy(map, newMap, map.Length);

            // РАНДОМНОЕ ДВИЖЕНИЕ ВРАГОВ
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (map[i, j] is Enemy enemy)
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
                        else if (newMap[newX, newY] is Hero hero)
                        {
                            hero.TakeDamage(10);
                            enemy.TakeDamage(15);
                            if (!hero.IsAlive) newMap[newX, newY] = new Field();
                            if (!enemy.IsAlive) newMap[i, j] = new Field();
                        }
                        else if (newMap[newX, newY] is Ally ally)
                        {
                            ally.TakeDamage(10);
                            enemy.TakeDamage(15);
                            if (!ally.IsAlive) newMap[newX, newY] = new Field();
                            if (!enemy.IsAlive) newMap[i, j] = new Field();
                        }
                    }
                }
            }
            Array.Copy(newMap, map, map.Length);
        }

        private void MoveAllies()
        {
            var allies = GetAllPersons().OfType<Ally>().ToList();
            var enemies = GetAllPersons().OfType<Enemy>().ToList();

            foreach (var ally in allies)
            {
                if (enemies.Count == 0) break;

                int ax = -1, ay = -1, ex = -1, ey = -1;
                int minDist = 9999;

                for (int i = 0; i < map.GetLength(0); i++)
                    for (int j = 0; j < map.GetLength(1); j++)
                        if (map[i, j] == ally) { ax = i; ay = j; }

                foreach (var enemy in enemies)
                {
                    for (int i = 0; i < map.GetLength(0); i++)
                        for (int j = 0; j < map.GetLength(1); j++)
                            if (map[i, j] == enemy)
                            {
                                int dist = Math.Abs(ax - i) + Math.Abs(ay - j);
                                if (dist < minDist) { minDist = dist; ex = i; ey = j; }
                            }
                }

                if (ex != -1 && ey != -1)
                {
                    if (minDist == 1)
                    {
                        Enemy target = map[ex, ey] as Enemy;
                        if (target != null)
                        {
                            target.TakeDamage(15);

                            // СОЮЗНИК ПОЛУЧАЕТ УРОН ТОЛЬКО ЕСЛИ ВРАГ НЕ ЗАМОРОЖЕН
                            if (ParalysisTurns == 0)
                            {
                                ally.TakeDamage(10);
                            }

                            if (!target.IsAlive) map[ex, ey] = new Field();
                            if (!ally.IsAlive) { map[ax, ay] = new Field(); break; }
                        }
                    }
                    else
                    {
                        int nx = ax, ny = ay;
                        if (Math.Abs(ax - ex) > Math.Abs(ay - ey)) nx = ax + (ex > ax ? 1 : -1);
                        else ny = ay + (ey > ay ? 1 : -1);

                        nx = (nx + map.GetLength(0)) % map.GetLength(0);
                        ny = (ny + map.GetLength(1)) % map.GetLength(1);

                        if (map[nx, ny] is Field)
                        {
                            map[nx, ny] = map[ax, ay];
                            map[ax, ay] = new Field();
                        }
                    }
                }
            }
        }

        internal IReadOnlyList<Person> GetAllPersons()
        {
            var persons = new List<Person>();
            for (int i = 0; i < map.GetLength(0); i++)
                for (int j = 0; j < map.GetLength(1); j++)
                    if (map[i, j] is Person p) persons.Add(p);
            return persons;
        }

        public bool HasLiveHero() => GetAllPersons().Any(p => p is Hero && p.IsAlive);

        public void CleanupDeadCharacters()
        {
            for (int i = 0; i < map.GetLength(0); i++)
                for (int j = 0; j < map.GetLength(1); j++)
                    if (map[i, j] is Person p && !p.IsAlive) map[i, j] = new Field();
        }

        internal SaveData ToSaveData()
        {
            var data = new SaveData();
            data.Width = map.GetLength(0); data.Height = map.GetLength(1);
            data.SavedAtUtc = DateTime.UtcNow.ToString("o");

            for (int i = 0; i < data.Width; i++)
                for (int j = 0; j < data.Height; j++)
                {
                    var obj = map[i, j];
                    var cell = new MapCellSaveData { X = i, Y = j };
                    if (obj is Wall) cell.Type = "Wall";
                    else if (obj is Tree) cell.Type = "Tree";
                    else if (obj is Hero h) { cell.Type = "Hero"; cell.Person = new PersonSaveData { Name = h.Name, Health = h.Health, MaxHealth = h.MaxHealth }; }
                    else if (obj is Enemy e) { cell.Type = "Enemy"; cell.Person = new PersonSaveData { Name = e.Name, Health = e.Health, MaxHealth = e.MaxHealth }; }
                    else if (obj is Ally a) { cell.Type = "Ally"; cell.Person = new PersonSaveData { Name = a.Name, Health = a.Health, MaxHealth = a.MaxHealth }; }
                    else cell.Type = "Field";
                    data.Cells.Add(cell);
                }
            return data;
        }

        internal void LoadFromSaveData(SaveData data)
        {
            map = new MapObject[data.Width, data.Height];
            bool heroLoaded = false;
            foreach (var cell in data.Cells)
            {
                switch (cell.Type)
                {
                    case "Wall": map[cell.X, cell.Y] = new Wall(); break;
                    case "Tree": map[cell.X, cell.Y] = new Tree(); break;
                    case "Hero":
                        if (!heroLoaded) { var h = new Hero(cell.X, cell.Y); if (cell.Person != null) h.Health = cell.Person.Health; map[cell.X, cell.Y] = h; heroLoaded = true; }
                        else map[cell.X, cell.Y] = new Field();
                        break;
                    case "Enemy":
                        var e = new Enemy(cell.X, cell.Y); if (cell.Person != null) { e.Health = cell.Person.Health; e.Name = cell.Person.Name; }
                        map[cell.X, cell.Y] = e;
                        break;
                    case "Ally":
                        var a = new Ally(cell.X, cell.Y); if (cell.Person != null) { a.Health = cell.Person.Health; }
                        map[cell.X, cell.Y] = a;
                        break;
                    default: map[cell.X, cell.Y] = new Field(); break;
                }
            }
        }
    }
}