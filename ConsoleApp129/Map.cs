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
        public bool LevelUpTriggered { get; set; } = false;

        public void Map_generation(Hero existingHero = null)
        {
            int enemyCount = 0;
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    map[i, j] = new Field();
                    int A = rand.Next(100);

                    if (A > 1 && A < 6) map[i, j] = new Wall();
                    else if (A < 3) { map[i, j] = new Enemy(i, j); enemyCount++; }
                    else if (A > 5 && A < 10) map[i, j] = new Tree();

                    // Если нам передали прокачанного Героя — ставим его. Иначе создаем нового.
                    if (i == 12 && j == 12)
                    {
                        if (existingHero != null) map[i, j] = existingHero;
                        else map[i, j] = new Hero(i, j);
                    }
                }
            }

            while (enemyCount < 7)
            {
                int rx = rand.Next(map.GetLength(0));
                int ry = rand.Next(map.GetLength(1));
                if (map[rx, ry] is Field && !(rx == 12 && ry == 12))
                {
                    map[rx, ry] = new Enemy(rx, ry); enemyCount++;
                }
            }
        }

        public void Drawing_the_map()
        {
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    Console.Write(map[i, j].RenderOnMap() + " ");
                    Console.ResetColor();
                }
                Console.WriteLine();
            }
        }

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

                        if (newMap[newX, newY] is Field || newMap[newX, newY] is Skull || newMap[newX, newY] is Portal)
                        {
                            if (newMap[newX, newY] is Skull) hero.InventorySkulls++;
                            if (newMap[newX, newY] is Portal) LevelUpTriggered = true;

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
                        else if (newMap[newX, newY] is Enemy enemy)
                        {
                            enemy.TakeDamage(hero.Damage);

                            if (ParalysisTurns == 0)
                            {
                                hero.TakeDamage(enemy.Damage);
                                Console.WriteLine("Вы ударили врага, он дал сдачи!");
                            }
                            else
                            {
                                Console.WriteLine("Вы избили замороженного врага!");
                            }
                            ConsoleHelper.Pause();

                            if (!enemy.IsAlive)
                            {
                                hero.Coins += 15; // МОНЕТЫ ЗА УБИЙСТВО
                                newMap[newX, newY] = new Skull();
                                newMap[i, j] = map[i, j];
                            }
                            if (!hero.IsAlive) newMap[i, j] = new Field();
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

            if (type == 'W' && map[tx, ty] is Field)
            {
                if (hero.InventoryWalls <= 0) throw new GameException("Нет стен!", "INV_WALL");
                map[tx, ty] = new Wall(); hero.InventoryWalls--; CheckPatterns();
            }
            else if (type == 'T' && map[tx, ty] is Field)
            {
                if (hero.InventoryTrees <= 0) throw new GameException("Нет деревьев!", "INV_TREE");
                map[tx, ty] = new Tree(); hero.InventoryTrees--; CheckPatterns();
            }
            else if (type == 'S' && map[tx, ty] is Field)
            {
                if (hero.InventorySkulls <= 0) throw new GameException("Нет черепов!", "INV_SKULL");
                map[tx, ty] = new Skull(); hero.InventorySkulls--; CheckPatterns();
            }
        }

        private void CheckPatterns()
        {
            var hero = GetAllPersons().FirstOrDefault(p => p is Hero) as Hero;
            if (hero == null) return;

            for (int i = 0; i < map.GetLength(0) - 2; i++)
            {
                for (int j = 0; j < map.GetLength(1) - 2; j++)
                {
                    // Крест из 5 стен (Большой +)
                    if (i > 0 && j > 0 && map[i, j] is Wall && map[i - 1, j] is Wall && map[i + 1, j] is Wall && map[i, j - 1] is Wall && map[i, j + 1] is Wall)
                    {
                        // Очищаем карту от стен, оставляя центр пустым
                        map[i, j] = new Field();
                        map[i, j - 1] = new Field();
                        map[i, j + 1] = new Field();

                        // Спавним союзников ПРЯМО на месте стен (сверху и снизу), чтобы они гарантированно появились
                        map[i - 1, j] = new Ally(i - 1, j, hero.Level, hero.BonusAllyHealth, hero.BonusAllyDamage);
                        map[i + 1, j] = new Ally(i + 1, j, hero.Level, hero.BonusAllyHealth, hero.BonusAllyDamage);

                        return;
                    }

                    // Буква Т из 5 деревьев (Заморозка)
                    if (i > 0 && j > 0 && map[i, j] is Tree && map[i - 1, j - 1] is Tree && map[i - 1, j] is Tree && map[i - 1, j + 1] is Tree && map[i + 1, j] is Tree)
                    {
                        map[i, j] = new Field();
                        map[i - 1, j - 1] = new Field();
                        map[i - 1, j] = new Field();
                        map[i - 1, j + 1] = new Field();
                        map[i + 1, j] = new Field();
                        ParalysisTurns = 30;
                        return;
                    }

                    // Портал "П" ИЗ 7 ЧЕРЕПОВ
                    if (map[i, j] is Skull && map[i, j + 1] is Skull && map[i, j + 2] is Skull &&
                        map[i + 1, j] is Skull && map[i + 1, j + 2] is Skull &&
                        map[i + 2, j] is Skull && map[i + 2, j + 2] is Skull)
                    {
                        map[i, j] = new Field(); map[i, j + 1] = new Field(); map[i, j + 2] = new Field();
                        map[i + 1, j] = new Field(); map[i + 1, j + 2] = new Field();
                        map[i + 2, j] = new Field(); map[i + 2, j + 2] = new Field();

                        if (map[i + 1, j + 1] is Hero)
                        {
                            LevelUpTriggered = true;
                        }
                        else
                        {
                            map[i + 1, j + 1] = new Portal();
                        }
                        return;
                    }
                }
            }
        }

        public void MovePersons()
        {
            if (ParalysisTurns > 0) { ParalysisTurns--; MoveAllies(); return; }
            MoveAllies();

            MapObject[,] newMap = new MapObject[map.GetLength(0), map.GetLength(1)];
            Array.Copy(map, newMap, map.Length);

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
                            hero.TakeDamage(enemy.Damage);
                            enemy.TakeDamage(hero.Damage);
                            if (!hero.IsAlive) newMap[newX, newY] = new Field();
                            if (!enemy.IsAlive) { hero.Coins += 15; newMap[i, j] = new Skull(); }
                        }
                        else if (newMap[newX, newY] is Ally ally)
                        {
                            ally.TakeDamage(enemy.Damage);
                            enemy.TakeDamage(ally.Damage);
                            if (!ally.IsAlive) newMap[newX, newY] = new Field();
                            if (!enemy.IsAlive)
                            {
                                var h = GetAllPersons().FirstOrDefault(p => p is Hero) as Hero;
                                if (h != null) h.Coins += 15; // МОНЕТЫ ЗА УБИЙСТВО СОЮЗНИКОМ
                                newMap[i, j] = new Skull();
                            }
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
                int ax = -1, ay = -1, ex = -1, ey = -1, minDist = 9999;

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
                            target.TakeDamage(ally.Damage);
                            if (ParalysisTurns == 0) ally.TakeDamage(target.Damage);
                            if (!target.IsAlive)
                            {
                                var h = GetAllPersons().FirstOrDefault(p => p is Hero) as Hero;
                                if (h != null) h.Coins += 15;
                                map[ex, ey] = new Skull();
                            }
                            if (!ally.IsAlive) { map[ax, ay] = new Field(); break; }
                        }
                    }
                    else
                    {
                        int[] nextStep = FindNextStepBFS(ax, ay, ex, ey);
                        if (nextStep != null && map[nextStep[0], nextStep[1]] is Field)
                        {
                            map[nextStep[0], nextStep[1]] = map[ax, ay];
                            map[ax, ay] = new Field();
                        }
                    }
                }
            }
        }

        private int[] FindNextStepBFS(int startX, int startY, int targetX, int targetY)
        {
            int w = map.GetLength(0), h = map.GetLength(1);
            bool[,] visited = new bool[w, h];
            int[,] prevX = new int[w, h], prevY = new int[w, h];
            for (int i = 0; i < w; i++) for (int j = 0; j < h; j++) { prevX[i, j] = -1; prevY[i, j] = -1; }

            Queue<int[]> q = new Queue<int[]>();
            q.Enqueue(new int[] { startX, startY });
            visited[startX, startY] = true;
            int[] dx = { -1, 1, 0, 0 }, dy = { 0, 0, -1, 1 };
            bool found = false;

            while (q.Count > 0)
            {
                int[] curr = q.Dequeue();
                if (curr[0] == targetX && curr[1] == targetY) { found = true; break; }

                for (int i = 0; i < 4; i++)
                {
                    int nx = (curr[0] + dx[i] + w) % w, ny = (curr[1] + dy[i] + h) % h;
                    if (!visited[nx, ny] && (map[nx, ny] is Field || (nx == targetX && ny == targetY)))
                    {
                        visited[nx, ny] = true;
                        prevX[nx, ny] = curr[0]; prevY[nx, ny] = curr[1];
                        q.Enqueue(new int[] { nx, ny });
                    }
                }
            }
            if (!found) return null;
            int px = targetX, py = targetY;
            while (prevX[px, py] != startX || prevY[px, py] != startY)
            {
                int tempX = prevX[px, py], tempY = prevY[px, py];
                px = tempX; py = tempY;
            }
            return new int[] { px, py };
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
                    else if (obj is Skull) cell.Type = "Skull";
                    else if (obj is Portal) cell.Type = "Portal";
                    else if (obj is Hero h) { cell.Type = "Hero"; cell.Person = new PersonSaveData { Name = h.Name, Health = h.Health, MaxHealth = h.MaxHealth, Coins = h.Coins }; }
                    else if (obj is Enemy e) { cell.Type = "Enemy"; cell.Person = new PersonSaveData { Name = e.Name, Health = e.Health, MaxHealth = e.MaxHealth }; }
                    else if (obj is Ally a) { cell.Type = "Ally"; cell.Person = new PersonSaveData { Name = a.Name, Health = a.Health, MaxHealth = a.MaxHealth }; }
                    else cell.Type = "Field";
                    data.Cells.Add(cell);
                }
            return data;
        }

        internal void LoadFromSaveData(SaveData data)
        {
            if (data == null) throw new GameException("Данные сохранения пусты!", "SAVE_DATA_NULL");

            map = new MapObject[data.Width, data.Height];
            bool heroLoaded = false;

            foreach (var cell in data.Cells)
            {
                switch (cell.Type)
                {
                    case "Wall": map[cell.X, cell.Y] = new Wall(); break;
                    case "Tree": map[cell.X, cell.Y] = new Tree(); break;
                    case "Skull": map[cell.X, cell.Y] = new Skull(); break;
                    case "Portal": map[cell.X, cell.Y] = new Portal(); break;
                    case "Hero":
                        if (!heroLoaded)
                        {
                            var h = new Hero(cell.X, cell.Y);
                            if (cell.Person != null) { h.Health = cell.Person.Health; h.Coins = cell.Person.Coins; }
                            map[cell.X, cell.Y] = h; heroLoaded = true;
                        }
                        else map[cell.X, cell.Y] = new Field();
                        break;
                    case "Enemy":
                        var e = new Enemy(cell.X, cell.Y); if (cell.Person != null) { e.Health = cell.Person.Health; e.Name = cell.Person.Name; }
                        map[cell.X, cell.Y] = e;
                        break;
                    case "Ally":
                        var a = new Ally(cell.X, cell.Y, 1, 0, 0); if (cell.Person != null) { a.Health = cell.Person.Health; }
                        map[cell.X, cell.Y] = a;
                        break;
                    default: map[cell.X, cell.Y] = new Field(); break;
                }
            }
            if (!heroLoaded) map[12, 12] = new Hero(12, 12);
        }
    }
}