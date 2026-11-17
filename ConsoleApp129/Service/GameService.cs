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
                    var deadHero = CurrentMap.GetAllPersons().FirstOrDefault(p => p is Hero) as Hero;
                    if (deadHero != null) StoryService.ShowDeathNarrative(deadHero.PortalHistory);
                    ConsoleHelper.Pause();
                    return;
                }

                var hero = CurrentMap.GetAllPersons().FirstOrDefault(p => p is Hero) as Hero;

                // ВХОД В СПЕЦ-УРОВЕНЬ
                if (CurrentMap.BossTriggered > 0 && !CurrentMap.IsBossMap)
                {
                    int artifactType = CurrentMap.BossTriggered;
                    CurrentMap.BossTriggered = 0;

                    StoryService.ShowPortalEntryStory(artifactType, hero.PortalHistory);
                    ConsoleHelper.Pause();

                    CurrentMap = new Map();
                    CurrentMap.GenerateSpecialLevel(artifactType, hero);
                    continue;
                }

                // ЗАВЕРШЕНИЕ СПЕЦ-УРОВНЯ
                if (CurrentMap.IsBossMap && CurrentMap.IsSpecialLevelComplete())
                {
                    int artifactType = CurrentMap.CurrentArtifactType;
                    hero.PortalHistory.Add(artifactType);
                    hero.BossDefeated[artifactType] = true;

                    StoryService.ShowPortalCompleteStory(artifactType, hero.PortalHistory);
                    ConsoleHelper.Pause();

                    bool allComplete = true;
                    for (int k = 1; k <= 5; k++)
                        if (!hero.BossDefeated[k]) { allComplete = false; break; }

                    if (allComplete)
                    {
                        StoryService.ShowEnding(hero.PortalHistory);
                        ConsoleHelper.Pause();
                        CurrentMap = null;
                        return;
                    }

                    // Уровень + магазин после прохождения спец-уровня
                    OpenShop(hero);
                    CurrentMap = new Map();
                    CurrentMap.Map_generation(hero);
                    continue;
                }

                Console.Clear();
                CurrentMap.CleanupDeadCharacters();

                if (CurrentMap.IsBossMap)
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("=== " + GetLevelTitle(CurrentMap.CurrentLevelType) + " ===");
                    Console.ResetColor();
                }

                CurrentMap.Drawing_the_map();
                Console.ResetColor();

                if (hero != null)
                {
                    int collected = 0;
                    for (int k = 1; k <= 5; k++) if (hero.BossDefeated[k]) collected++;

                    Console.WriteLine("Ур:{0} HP:{1}/{2} Монеты:{3} Печати:{4}/5 | [+] {5} [T] {6} [☠] {7}",
                        hero.Level, hero.Health, hero.MaxHealth, hero.Coins, collected,
                        hero.InventoryWalls, hero.InventoryTrees, hero.InventorySkulls);

                    string arts = "Артефакты: ";
                    for (int t = 1; t <= 5; t++)
                    {
                        string prefix = (t == hero.SelectedArtifactType) ? ">" : "";
                        string suffix = (t == hero.SelectedArtifactType) ? "<" : "";
                        arts += string.Format("{0}{1}:{2}{3} ", prefix,
                            ArtifactInfo.GetSymbol(t), hero.InventoryArtifacts[t], suffix);
                    }
                    Console.WriteLine(arts);
                }

                Console.WriteLine("Стрелки-ход, 1=+, 2=T, 3=☠, 4=♦(выбранная), C=Меню");

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

                    if (CurrentMap.LevelUpTriggered && !CurrentMap.IsBossMap && hero != null)
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

        private static string GetLevelTitle(LevelType t)
        {
            switch (t)
            {
                case LevelType.ChaosBoss: return "ЛОГОВО ВЛАДЫКИ ПЛАМЕНИ";
                case LevelType.OrderBoss: return "ЧЕРТОГИ ЛЕДЯНОГО СТРАЖА";
                case LevelType.Arena: return "ЗАЛ ИСПЫТАНИЙ";
                case LevelType.Abyss: return "БЕЗДНА";
                case LevelType.Sanctuary: return "СВЯТИЛИЩЕ";
                default: return "";
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
                        if (hero.Coins >= 30) { hero.Coins -= 30; hero.BonusAllyHealth += 20; Console.WriteLine("Союзники станут крепче!"); }
                        else Console.WriteLine("Недостаточно монет!");
                        break;
                    case "3":
                        if (hero.Coins >= 30) { hero.Coins -= 30; hero.BonusAllyDamage += 5; Console.WriteLine("Союзники станут сильнее!"); }
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