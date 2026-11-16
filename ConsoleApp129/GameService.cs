using ConsoleApp129.Core;
using ConsoleApp129.Menus;
using ConsoleApp129.SaveSystem;
using System;
using System.Linq;


namespace ConsoleApp129.Services
{
    /// Сервис, управляющий игровым процессом (цикл игры, магазин).
    internal sealed class GameService
    {
        public Map CurrentMap { get; private set; }
        public bool IsGameActive { get { return CurrentMap != null; } }

        /// Начинает новую игру и генерирует карту.
        public void StartNewGame()
        {
            CurrentMap = new Map();
            CurrentMap.Map_generation();
        }

        /// Запускает основной цикл игры.
        /// Ссылка на меню персонажей.
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

                Console.Clear();
                CurrentMap.CleanupDeadCharacters();
                CurrentMap.Drawing_the_map();

                var hero = CurrentMap.GetAllPersons().FirstOrDefault(p => p is Hero) as Hero;
                string invInfo = hero != null ? string.Format("[+] {0}  [T] {1}  [☠] {2}  [Монеты] {3}", hero.InventoryWalls, hero.InventoryTrees, hero.InventorySkulls, hero.Coins) : "";
                string freezeInfo = CurrentMap.ParalysisTurns > 0 ? string.Format(" (Заморозка: {0})", CurrentMap.ParalysisTurns) : "";

                Console.WriteLine("\nУровень: " + (hero != null ? hero.Level : 1) + " | HP: " + (hero != null ? hero.Health : 0) + " | Урон: " + (hero != null ? hero.Damage : 0));
                Console.WriteLine("Инвентарь: " + invInfo + freezeInfo);
                Console.WriteLine("Стрелки - ход/атака, 1 - стена(+), 2 - дерево(T), 3 - поставить череп(☠), C - персонажи");

                while (Console.KeyAvailable) Console.ReadKey(true);
                var keyInfo = Console.ReadKey(true);

                if (keyInfo.Key == ConsoleKey.Escape) return;
                if (keyInfo.Key == ConsoleKey.C) { characterMenu.Show(); continue; }

                try
                {
                    if (keyInfo.Key == ConsoleKey.D1 || keyInfo.Key == ConsoleKey.NumPad1) { CurrentMap.PlaceObject('W'); continue; }
                    if (keyInfo.Key == ConsoleKey.D2 || keyInfo.Key == ConsoleKey.NumPad2) { CurrentMap.PlaceObject('T'); continue; }
                    if (keyInfo.Key == ConsoleKey.D3 || keyInfo.Key == ConsoleKey.NumPad3) { CurrentMap.PlaceObject('S'); continue; }

                    CurrentMap.MovePersons(keyInfo.Key);
                    CurrentMap.MovePersons();

                    if (CurrentMap.LevelUpTriggered && hero != null)
                    {
                        OpenShop(hero);
                        CurrentMap = new Map();
                        CurrentMap.Map_generation(hero); // ПЕРЕДАЕМ ПРОКАЧАННОГО ГЕРОЯ!
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
        /// Открывает магазин прокачки после перехода на новый уровень.
        /// Ссылка на прокачиваемого Героя.
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
                Console.WriteLine("2. Урон Героя (+10 Урона) - 50 монет");
                Console.WriteLine("3. Здоровье Союзников (+20 HP) - 30 монет");
                Console.WriteLine("4. Урон Союзников (+5 Урона) - 30 монет");
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
                        if (hero.Coins >= 50) { hero.Coins -= 50; hero.Damage += 10; Console.WriteLine("Урон Героя увеличен!"); }
                        else Console.WriteLine("Недостаточно монет!");
                        break;
                    case "3":
                        if (hero.Coins >= 30) { hero.Coins -= 30; hero.BonusAllyHealth += 20; Console.WriteLine("Будущие союзники станут крепче!"); }
                        else Console.WriteLine("Недостаточно монет!");
                        break;
                    case "4":
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