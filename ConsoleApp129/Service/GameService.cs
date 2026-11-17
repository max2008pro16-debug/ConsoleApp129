using ConsoleApp129.Core;
using ConsoleApp129.Menus;
using System;
using System.Linq;

namespace ConsoleApp129.Services
{
    internal sealed class GameService
    {
        public Map CurrentMap { get; private set; }
        public bool IsGameActive { get { return CurrentMap != null; } }

        public void StartNewGame()
        {
            CurrentMap = new Map();
            CurrentMap.Map_generation();
        }

        public void RunGameLoop(IMenu characterMenu)
        {
            if (CurrentMap == null) return;

            while (true)
            {
                if (!CurrentMap.HasLiveHero())
                {
                    Console.Clear();
                    Console.WriteLine("Герой погиб! Игра окончена.");
                    ConsoleHelper.Pause();
                    return;
                }

                var hero = CurrentMap.GetAllPersons().FirstOrDefault(p => p is Hero) as Hero;

                if (CurrentMap.BossTriggered > 0 && !CurrentMap.IsBossMap)
                {
                    int bossType = CurrentMap.BossTriggered;
                    Console.Clear();
                    Console.WriteLine("Вы построили портал к Боссу " + bossType + "!");
                    ConsoleHelper.Pause();

                    CurrentMap = new Map();
                    CurrentMap.GenerateBossMap(bossType, hero);
                    continue;
                }

                if (CurrentMap.IsBossMap)
                {
                    var boss = CurrentMap.GetAllPersons().OfType<Boss>().FirstOrDefault();
                    if (boss == null || !boss.IsAlive)
                    {
                        Console.Clear();
                        Console.WriteLine("Вы победили Босса " + CurrentMap.CurrentBossType + "!");
                        hero.BossDefeated[CurrentMap.CurrentBossType] = true;
                        hero.InventoryArtifacts[CurrentMap.CurrentBossType] = 0;

                        int defeatedCount = 0;
                        for (int k = 1; k <= 5; k++) if (hero.BossDefeated[k]) defeatedCount++;

                        if (defeatedCount >= 5)
                        {
                            Console.Clear();
                            Console.WriteLine("ПОЗДРАВЛЯЕМ! ВЫ ПОБЕДИЛИ ВСЕХ 5 БОССОВ И ПРОШЛИ ИГРУ!");
                            ConsoleHelper.Pause();
                            CurrentMap = null;
                            return;
                        }
                        else
                        {
                            ConsoleHelper.Pause();
                            CurrentMap = new Map();
                            CurrentMap.Map_generation(hero);
                            continue;
                        }
                    }
                }

                Console.Clear();
                CurrentMap.CleanupDeadCharacters();
                CurrentMap.Drawing_the_map();
                Console.ResetColor();

                if (hero != null)
                {
                    string stats = string.Format("Ур:{0} HP:{1}/{2} Монеты:{3} | [+] {4} [T] {5} [☠] {6}",
                        hero.Level, hero.Health, hero.MaxHealth, hero.Coins,
                        hero.InventoryWalls, hero.InventoryTrees, hero.InventorySkulls);
                    Console.WriteLine(stats);

                    string arts = "Артефакты: ";
                    for (int t = 1; t <= 5; t++)
                    {
                        if (t == hero.SelectedArtifactType)
                            arts += string.Format(">[{0}:{1}]< ", t, hero.InventoryArtifacts[t]);
                        else
                            arts += string.Format("[{0}:{1}] ", t, hero.InventoryArtifacts[t]);
                    }
                    Console.WriteLine(arts);
                }

                Console.WriteLine("Стрелки-ход, 1=+, 2=T, 3=☠, 4=♦, C=Меню");

                while (Console.KeyAvailable) Console.ReadKey(true);
                var keyInfo = Console.ReadKey(true);

                if (keyInfo.Key == ConsoleKey.Escape) return;
                if (keyInfo.Key == ConsoleKey.C) { characterMenu.Show(); continue; }

                try
                {
                    if (keyInfo.Key == ConsoleKey.D1 || keyInfo.Key == ConsoleKey.NumPad1) { CurrentMap.PlaceObject('W'); continue; }
                    if (keyInfo.Key == ConsoleKey.D2 || keyInfo.Key == ConsoleKey.NumPad2) { CurrentMap.PlaceObject('T'); continue; }
                    if (keyInfo.Key == ConsoleKey.D3 || keyInfo.Key == ConsoleKey.NumPad3) { CurrentMap.PlaceObject('S'); continue; }
                    if (keyInfo.Key == ConsoleKey.D4 || keyInfo.Key == ConsoleKey.NumPad4) { CurrentMap.PlaceObject('A'); continue; }

                    CurrentMap.MovePersons(keyInfo.Key);
                    CurrentMap.MovePersons();

                    if (CurrentMap.LevelUpTriggered && hero != null)
                    {
                        OpenShop(hero);
                        CurrentMap = new Map();
                        CurrentMap.Map_generation(hero);
                    }
                }
                catch (GameException ex)
                {
                    Console.WriteLine();
                    Console.WriteLine(string.Format("Ошибка [{0}]: {1}", ex.ErrorCode, ex.Message));
                    ConsoleHelper.Pause();
                }
            }
        }

        private void OpenShop(Hero hero)
        {
            hero.Level++;
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== ПЕРЕХОД НА НОВЫЙ УРОВЕНЬ: МАГАЗИН ===");
                Console.WriteLine("Ваш уровень: " + hero.Level);
                Console.WriteLine("Ваши монеты: " + hero.Coins);
                Console.WriteLine("---------------------------------");
                Console.WriteLine("1. Здоровье Героя (+50 HP) - 50 монет");
                Console.WriteLine("2. Здоровье Союзников (+20 HP) - 30 монет");
                Console.WriteLine("3. Урон Союзников (+5 Урона) - 30 монет");
                Console.WriteLine("0. Закончить покупки и перейти на следующий уровень");
                Console.Write("Ваш выбор: ");

                string choice = Console.ReadLine();
                if (choice == "0") break;

                switch (choice)
                {
                    case "1":
                        if (hero.Coins >= 50) { hero.Coins -= 50; hero.MaxHealth += 50; hero.Health = hero.MaxHealth; Console.WriteLine("Здоровье Героя увеличено!"); }
                        else Console.WriteLine("Недостаточно монет!");
                        break;
                    case "2":
                        if (hero.Coins >= 30) { hero.Coins -= 30; hero.BonusAllyHealth += 20; Console.WriteLine("Будущие союзники станут крепче!"); }
                        else Console.WriteLine("Недостаточно монет!");
                        break;
                    case "3":
                        if (hero.Coins >= 30) { hero.Coins -= 30; hero.BonusAllyDamage += 5; Console.WriteLine("Будущие союзники станут сильнее!"); }
                        else Console.WriteLine("Недостаточно монет!");
                        break;
                    default: Console.WriteLine("Неверный ввод."); break;
                }
                Console.WriteLine("Нажмите Enter...");
                Console.ReadLine();
            }
        }

        public void SetMap(Map map)
        {
            if (map == null) throw new ArgumentNullException(nameof(map));
            CurrentMap = map;
        }
    }
}