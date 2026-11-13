using ConsoleApp129.Core;
using ConsoleApp129.Menus;
using ConsoleApp129.SaveSystem;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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

                Console.Clear();
                CurrentMap.CleanupDeadCharacters();
                CurrentMap.Drawing_the_map();

                var hero = CurrentMap.GetAllPersons().FirstOrDefault(p => p is Hero) as Hero;
                string invInfo = hero != null ? string.Format("[+] {0}  [T] {1}", hero.InventoryWalls, hero.InventoryTrees) : "";
                string freezeInfo = CurrentMap.ParalysisTurns > 0 ? string.Format(" (Заморозка: {0} ходов)", CurrentMap.ParalysisTurns) : "";

                Console.WriteLine("\nИнвентарь: " + invInfo + freezeInfo);
                Console.WriteLine("Стрелки - ход/атака врага, 1 - поставить +, 2 - поставить T, C - персонажи, Esc - меню");

                var keyInfo = Console.ReadKey(true);

                if (keyInfo.Key == ConsoleKey.Escape) return;

                if (keyInfo.Key == ConsoleKey.C)
                {
                    characterMenu.Show();
                    continue;
                }

                if (keyInfo.Key == ConsoleKey.D1 || keyInfo.Key == ConsoleKey.NumPad1)
                {
                    CurrentMap.PlaceObject('W');
                    continue;
                }

                if (keyInfo.Key == ConsoleKey.D2 || keyInfo.Key == ConsoleKey.NumPad2)
                {
                    CurrentMap.PlaceObject('T');
                    continue;
                }

                // Ход Героя (и его атака, если он шагнул на врага)
                CurrentMap.MovePersons(keyInfo.Key);
                // Ход Врагов и Союзников
                CurrentMap.MovePersons();
            }
        }

        public void SetMap(Map map)
        {
            if (map == null) throw new ArgumentNullException(nameof(map));
            CurrentMap = map;
        }
    }
}