using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleApp129.Core;
using ConsoleApp129.SaveSystem;

namespace ConsoleApp129
{
    internal class Map
    {
        Random rand = new Random();
        MapObject[,] map = new MapObject[25, 25];

        public int ParalysisTurns { get; set; } = 0;
        public bool LevelUpTriggered { get; set; } = false;
        public bool GameWon { get; set; } = false;

        public int BossTriggered { get; set; } = 0;
        public bool IsBossMap { get; set; } = false;
        public int CurrentBossType { get; set; } = 0;

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
                    else if (A > 5 && A < 10) map[i, j] = new Tree();

                    if (i == 12 && j == 12)
                    {
                        if (existingHero != null) map[i, j] = existingHero;
                        else map[i, j] = new Hero(i, j);
                    }
                }
            }

            int currentLevel = existingHero != null ? existingHero.Level : 1;
            int enemiesToSpawn = rand.Next(7, 16);
            int attempts = 0;

            while (enemyCount < enemiesToSpawn && attempts < 1000)
            {
                int rx = rand.Next(map.GetLength(0));
                int ry = rand.Next(map.GetLength(1));
                if (map[rx, ry] is Field && !(rx == 12 && ry == 12))
                {
                    int type = rand.Next(100);
                    if (type < 50) map[rx, ry] = new Enemy(rx, ry);
                    else if (type < 75 && currentLevel >= 2) map[rx, ry] = new Hunter(rx, ry);
                    else if (type < 90 && currentLevel >= 3) map[rx, ry] = new Archer(rx, ry);
                    else map[rx, ry] = new Enemy(rx, ry);
                    enemyCount++;
                }
                attempts++;
            }

            if (existingHero != null)
            {
                System.Collections.Generic.List<int> availableTypes = new System.Collections.Generic.List<int>();
                for (int t = 1; t <= 5; t++)
                {
                    if (!existingHero.BossDefeated[t]) availableTypes.Add(t);
                }

                if (availableTypes.Count > 0)
                {
                    if (rand.Next(100) < 50 && availableTypes.Count >= 2)
                    {
                        int t1 = availableTypes[rand.Next(availableTypes.Count)];
                        int t2 = availableTypes[rand.Next(availableTypes.Count)];
                        while (t2 == t1) t2 = availableTypes[rand.Next(availableTypes.Count)];

                        int rx1 = rand.Next(25); int ry1 = rand.Next(25);
                        if (map[rx1, ry1] is Field) map[rx1, ry1] = new Artifact(t1);

                        int rx2 = rand.Next(25); int ry2 = rand.Next(25);
                        if (map[rx2, ry2] is Field) map[rx2, ry2] = new Artifact(t2);
                    }
                    else
                    {
                        int typeToSpawn = availableTypes[rand.Next(availableTypes.Count)];
                        int amount = rand.Next(1, 3);
                        for (int a = 0; a < amount; a++)
                        {
                            int rx = rand.Next(25); int ry = rand.Next(25);
                            if (map[rx, ry] is Field) map[rx, ry] = new Artifact(typeToSpawn);
                        }
                    }
                }
            }
        }

        public void GenerateBossMap(int bossType, Hero hero)
        {
            map = new MapObject[25, 25];
            IsBossMap = true;
            CurrentBossType = bossType;

            for (int i = 0; i < 25; i++)
            {
                for (int j = 0; j < 25; j++)
                {
                    map[i, j] = new Field();
                    if (i == 0 || i == 24 || j == 0 || j == 24) map[i, j] = new Wall();
                    else if (rand.Next(100) < 10) map[i, j] = new Wall();
                }
            }
            map[22, 12] = hero;
            map[2, 12] = new Boss(2, 12, bossType);
        }

        public void Drawing_the_map()
        {
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    Console.Write(map[i, j].RenderOnMap());
                    Console.Write(" ");
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

                        if (newMap[newX, newY] is Field || newMap[newX, newY] is Skull || newMap[newX, newY] is Portal || newMap[newX, newY] is Artifact)
                        {
                            if (newMap[newX, newY] is Skull) hero.InventorySkulls++;
                            if (newMap[newX, newY] is Portal) LevelUpTriggered = true;

                            if (newMap[newX, newY] is Artifact ar)
                            {
                                hero.InventoryArtifacts[ar.Type]++;
                                Console.WriteLine("Вы нашли Артефакт " + ar.Type + "!");
                                ConsoleHelper.Pause();
                            }

                            newMap[newX, newY] = map[i, j];
                            newMap[i, j] = new Field();
                        }
                        else if (newMap[newX, newY] is Wall)
                        {
                            int amount = 1;
                            if (rand.Next(100) < 25)
                            {
                                amount = 2;
                                Console.WriteLine("Повезло! Вы нашли 2 стены (+)");
                                ConsoleHelper.Pause();
                            }
                            hero.InventoryWalls += amount;
                            newMap[newX, newY] = map[i, j];
                            newMap[i, j] = new Field();
                        }
                        else if (newMap[newX, newY] is Tree)
                        {
                            hero.InventoryTrees++;
                            newMap[newX, newY] = map[i, j];
                            newMap[i, j] = new Field();
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
            else if (type == 'A' && map[tx, ty] is Field)
            {
                if (hero.InventoryArtifacts[hero.SelectedArtifactType] > 0)
                {
                    map[tx, ty] = new Artifact(hero.SelectedArtifactType);
                    hero.InventoryArtifacts[hero.SelectedArtifactType]--;
                    CheckPatterns();
                }
                else
                {
                    bool found = false;
                    for (int t = 1; t <= 5; t++)
                    {
                        if (hero.InventoryArtifacts[t] > 0)
                        {
                            hero.SelectedArtifactType = t;
                            map[tx, ty] = new Artifact(t);
                            hero.InventoryArtifacts[t]--;
                            CheckPatterns();
                            found = true;
                            break;
                        }
                    }
                    if (!found) throw new GameException("Нет артефактов!", "INV_ARTIFACT");
                }
            }
        }

        private void CheckPatterns()
        {
            var hero = GetAllPersons().FirstOrDefault(p => p is Hero) as Hero;
            if (hero == null) return;

            for (int i = 1; i < map.GetLength(0) - 2; i++)
            {
                for (int j = 1; j < map.GetLength(1) - 2; j++)
                {
                    if (map[i, j] is Wall && map[i - 1, j] is Wall && map[i + 1, j] is Wall && map[i, j - 1] is Wall && map[i, j + 1] is Wall)
                    {
                        map[i, j] = new Field(); map[i - 1, j] = new Field(); map[i + 1, j] = new Field();
                        map[i, j - 1] = new Field(); map[i, j + 1] = new Field();

                        int alliesSpawned = 0;
                        for (int dx = -1; dx <= 1 && alliesSpawned < 2; dx++)
                        {
                            for (int dy = -1; dy <= 1 && alliesSpawned < 2; dy++)
                            {
                                int nx = i + dx; int ny = j + dy;
                                if (map[nx, ny] is Field)
                                {
                                    map[nx, ny] = new Ally(nx, ny, hero.Level, hero.BonusAllyHealth, hero.BonusAllyDamage);
                                    alliesSpawned++;
                                }
                            }
                        }
                        return;
                    }

                    if (map[i, j] is Tree && map[i - 1, j - 1] is Tree && map[i - 1, j] is Tree && map[i - 1, j + 1] is Tree && map[i + 1, j] is Tree)
                    {
                        map[i, j] = new Field(); map[i - 1, j - 1] = new Field(); map[i - 1, j] = new Field();
                        map[i - 1, j + 1] = new Field(); map[i + 1, j] = new Field();
                        ParalysisTurns = 30; return;
                    }

                    if (map[i, j] is Skull && map[i, j + 1] is Skull && map[i, j + 2] is Skull &&
                        map[i + 1, j] is Skull && map[i + 1, j + 2] is Skull &&
                        map[i + 2, j] is Skull && map[i + 2, j + 2] is Skull)
                    {
                        map[i, j] = new Field(); map[i, j + 1] = new Field(); map[i, j + 2] = new Field();
                        map[i + 1, j] = new Field(); map[i + 1, j + 2] = new Field();
                        map[i + 2, j] = new Field(); map[i + 2, j + 2] = new Field();
                        if (map[i + 1, j + 1] is Hero) LevelUpTriggered = true;
                        else map[i + 1, j + 1] = new Portal();
                        return;
                    }

                    for (int t = 1; t <= 5; t++)
                    {
                        if (map[i, j] is Artifact a1 && a1.Type == t &&
                            map[i, j + 1] is Artifact a2 && a2.Type == t &&
                            map[i, j + 2] is Artifact a3 && a3.Type == t &&
                            map[i + 1, j] is Artifact a4 && a4.Type == t &&
                            map[i + 1, j + 2] is Artifact a5 && a5.Type == t &&
                            map[i + 2, j] is Artifact a6 && a6.Type == t &&
                            map[i + 2, j + 2] is Artifact a7 && a7.Type == t)
                        {
                            map[i, j] = new Field(); map[i, j + 1] = new Field(); map[i, j + 2] = new Field();
                            map[i + 1, j] = new Field(); map[i + 1, j + 2] = new Field();
                            map[i + 2, j] = new Field(); map[i + 2, j + 2] = new Field();
                            BossTriggered = t;
                            return;
                        }
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

            var hero = GetAllPersons().FirstOrDefault(p => p is Hero) as Hero;
            int heroX = -1, heroY = -1;
            if (hero != null)
            {
                for (int i = 0; i < map.GetLength(0); i++)
                    for (int j = 0; j < map.GetLength(1); j++)
                        if (map[i, j] == hero) { heroX = i; heroY = j; }
            }

            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (map[i, j] is Enemy || map[i, j] is Hunter || map[i, j] is Archer || map[i, j] is Boss)
                    {
                        Person enemy = (Person)map[i, j];
                        bool actionTaken = false;

                        if (enemy is Archer)
                        {
                            int[] target = FindNearestTarget(i, j);
                            if (target != null)
                            {
                                int dist = Math.Abs(i - target[0]) + Math.Abs(j - target[1]);
                                if (dist <= 4 && (i == target[0] || j == target[1]))
                                {
                                    Person victim = map[target[0], target[1]] as Person;
                                    if (victim != null)
                                    {
                                        victim.TakeDamage(enemy.Damage);

                                        // ПАУЗА ТОЛЬКО ПРИ СМЕРТИ ИЛИ ЕСЛИ 1 УДАР ОСТАЛСЯ
                                        if (victim.Health <= 0)
                                        {
                                            Console.WriteLine("Лучник убил " + victim.Name + "!");
                                            ConsoleHelper.Pause();
                                        }
                                        else if (victim.Health <= enemy.Damage)
                                        {
                                            Console.WriteLine("ОПАСНО! Лучник ранил " + victim.Name + ". Осталось мало HP: " + victim.Health);
                                            ConsoleHelper.Pause();
                                        }
                                    }
                                    if (victim != null && !victim.IsAlive) newMap[target[0], target[1]] = new Field();
                                    actionTaken = true;
                                }
                                else if (dist <= 2)
                                {
                                    int stepX = i, stepY = j;
                                    if (Math.Abs(i - target[0]) > Math.Abs(j - target[1]))
                                        stepX = i + (target[0] > i ? -1 : 1);
                                    else
                                        stepY = j + (target[1] > j ? -1 : 1);

                                    stepX = (stepX + map.GetLength(0)) % map.GetLength(0);
                                    stepY = (stepY + map.GetLength(1)) % map.GetLength(1);

                                    if (newMap[stepX, stepY] is Field)
                                    {
                                        newMap[stepX, stepY] = map[i, j];
                                        newMap[i, j] = new Field();
                                    }
                                    actionTaken = true;
                                }
                            }
                        }

                        if (actionTaken) continue;

                        int newX = i, newY = j;

                        if (enemy is Hunter || enemy is Boss)
                        {
                            int[] target = FindNearestTarget(i, j);
                            if (target != null)
                            {
                                int[] step = FindNextStepBFS(i, j, target[0], target[1]);
                                if (step != null) { newX = step[0]; newY = step[1]; }
                            }
                        }
                        else
                        {
                            int direction = rand.Next(4);
                            switch (direction)
                            {
                                case 0: newX = (i - 1 + map.GetLength(0)) % map.GetLength(0); break;
                                case 1: newX = (i + 1) % map.GetLength(0); break;
                                case 2: newY = (j - 1 + map.GetLength(1)) % map.GetLength(1); break;
                                case 3: newY = (j + 1) % map.GetLength(1); break;
                            }
                        }

                        if (newMap[newX, newY] is Field)
                        {
                            newMap[newX, newY] = map[i, j];
                            newMap[i, j] = new Field();
                        }
                        else if (newMap[newX, newY] is Hero h)
                        {
                            h.TakeDamage(enemy.Damage);

                            // ПАУЗА ТОЛЬКО ПРИ СМЕРТИ ИЛИ ЕСЛИ 1 УДАР ОСТАЛСЯ
                            if (h.Health <= 0)
                            {
                                Console.WriteLine("Герой погиб от удара!");
                                ConsoleHelper.Pause();
                            }
                            else if (h.Health <= enemy.Damage)
                            {
                                Console.WriteLine("ВНИМАНИЕ! У Героя осталось мало HP (1 удар): " + h.Health);
                                ConsoleHelper.Pause();
                            }

                            if (!h.IsAlive) newMap[newX, newY] = new Field();
                        }
                        else if (newMap[newX, newY] is Ally ally)
                        {
                            ally.TakeDamage(enemy.Damage);
                            enemy.TakeDamage(ally.Damage);

                            // ПАУЗА ТОЛЬКО ПРИ СМЕРТИ ИЛИ ЕСЛИ 1 УДАР ОСТАЛСЯ
                            if (ally.Health <= 0)
                            {
                                Console.WriteLine("Враг убил Союзника!");
                                ConsoleHelper.Pause();
                            }
                            else if (ally.Health <= enemy.Damage)
                            {
                                Console.WriteLine("ВНИМАНИЕ! Союзник при смерти (1 удар, HP: " + ally.Health + ")");
                                ConsoleHelper.Pause();
                            }

                            if (!ally.IsAlive) newMap[newX, newY] = new Field();
                            if (!enemy.IsAlive) { newMap[i, j] = new Skull(); }
                        }
                    }
                }
            }
            Array.Copy(newMap, map, map.Length);
        }

        private int[] FindNearestTarget(int startX, int startY)
        {
            int minDist = 9999;
            Person targetPerson = null;
            int tx = -1, ty = -1;

            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (map[i, j] is Hero || map[i, j] is Ally)
                    {
                        int dist = Math.Abs(startX - i) + Math.Abs(startY - j);
                        if (dist < minDist) { minDist = dist; tx = i; ty = j; targetPerson = (Person)map[i, j]; }
                    }
                }
            }
            if (targetPerson != null) return new int[] { tx, ty };
            return null;
        }

        private void MoveAllies()
        {
            var allies = GetAllPersons().OfType<Ally>().ToList();
            var enemies = GetAllPersons().Where(p => p is Enemy || p is Hunter || p is Archer || p is Boss).ToList();
            int steps = ParalysisTurns > 0 ? 2 : 1;

            foreach (var ally in allies)
            {
                for (int step = 0; step < steps; step++)
                {
                    if (enemies.Count == 0 || !ally.IsAlive) break;
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
                            Person target = map[ex, ey] as Person;
                            if (target != null)
                            {
                                target.TakeDamage(ally.Damage);

                                // ЕСЛИ ВРАГ НЕ ЗАМОРОЖЕН, ОН БЬЕТ В ОТВЕТ
                                if (ParalysisTurns == 0)
                                {
                                    ally.TakeDamage(target.Damage);

                                    // ПАУЗА ТОЛЬКО ПРИ СМЕРТИ ИЛИ ЕСЛИ 1 УДАР ОСТАЛСЯ
                                    if (ally.Health <= 0)
                                    {
                                        Console.WriteLine("Союзник погиб в бою!");
                                        ConsoleHelper.Pause();
                                    }
                                    else if (ally.Health <= target.Damage)
                                    {
                                        Console.WriteLine("ВНИМАНИЕ! Союзник при смерти (1 удар, HP: " + ally.Health + ")");
                                        ConsoleHelper.Pause();
                                    }
                                }
                                // Если враг заморожен, никакой паузы не будет (союзник просто бьет)

                                if (!target.IsAlive)
                                {
                                    var h = GetAllPersons().FirstOrDefault(p => p is Hero) as Hero;
                                    if (h != null && !(target is Boss)) h.Coins += 15;
                                    map[ex, ey] = new Skull();
                                    enemies.Remove(target);
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
                            else break;
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
                    else if (obj is Artifact ar) { cell.Type = "Artifact"; cell.ArtifactType = ar.Type; }
                    else if (obj is Hero h) { cell.Type = "Hero"; cell.Person = new PersonSaveData { Name = h.Name, Health = h.Health, MaxHealth = h.MaxHealth, Coins = h.Coins, InventoryArtifacts = h.InventoryArtifacts, BossDefeated = h.BossDefeated }; }
                    else if (obj is Hunter hn) { cell.Type = "Hunter"; cell.Person = new PersonSaveData { Name = hn.Name, Health = hn.Health, MaxHealth = hn.MaxHealth }; }
                    else if (obj is Archer a) { cell.Type = "Archer"; cell.Person = new PersonSaveData { Name = a.Name, Health = a.Health, MaxHealth = a.MaxHealth }; }
                    else if (obj is Enemy e) { cell.Type = "Enemy"; cell.Person = new PersonSaveData { Name = e.Name, Health = e.Health, MaxHealth = e.MaxHealth }; }
                    else if (obj is Ally al) { cell.Type = "Ally"; cell.Person = new PersonSaveData { Name = al.Name, Health = al.Health, MaxHealth = al.MaxHealth }; }
                    else if (obj is Boss b) { cell.Type = "Boss"; cell.Person = new PersonSaveData { Name = b.Name, Health = b.Health, MaxHealth = b.MaxHealth }; }
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
                    case "Artifact": map[cell.X, cell.Y] = new Artifact(cell.ArtifactType); break;
                    case "Boss":
                        var b = new Boss(cell.X, cell.Y, 1); if (cell.Person != null) { b.Health = cell.Person.Health; }
                        map[cell.X, cell.Y] = b;
                        break;
                    case "Hero":
                        if (!heroLoaded)
                        {
                            var h = new Hero(cell.X, cell.Y);
                            if (cell.Person != null) { h.Health = cell.Person.Health; h.Coins = cell.Person.Coins; h.InventoryArtifacts = cell.Person.InventoryArtifacts; h.BossDefeated = cell.Person.BossDefeated; }
                            map[cell.X, cell.Y] = h; heroLoaded = true;
                        }
                        else map[cell.X, cell.Y] = new Field();
                        break;
                    case "Enemy":
                        var e = new Enemy(cell.X, cell.Y); if (cell.Person != null) { e.Health = cell.Person.Health; }
                        map[cell.X, cell.Y] = e;
                        break;
                    case "Hunter":
                        var hn = new Hunter(cell.X, cell.Y); if (cell.Person != null) { hn.Health = cell.Person.Health; }
                        map[cell.X, cell.Y] = hn;
                        break;
                    case "Archer":
                        var a = new Archer(cell.X, cell.Y); if (cell.Person != null) { a.Health = cell.Person.Health; }
                        map[cell.X, cell.Y] = a;
                        break;
                    case "Ally":
                        var al = new Ally(cell.X, cell.Y, 1, 0, 0); if (cell.Person != null) { al.Health = cell.Person.Health; }
                        map[cell.X, cell.Y] = al;
                        break;
                    default: map[cell.X, cell.Y] = new Field(); break;
                }
            }
            if (!heroLoaded) map[12, 12] = new Hero(12, 12);
        }
    }
}