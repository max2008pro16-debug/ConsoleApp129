using ConsoleApp129.Core;
using ConsoleApp129.Menus;
using ConsoleApp129.SaveSystem;
using System;
using System.Collections.Generic;
using System.Linq;
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

                Console.WriteLine("\nСтрелки - движение, C - персонажи, Esc - в меню");
                var keyInfo = Console.ReadKey(true);

                if (keyInfo.Key == ConsoleKey.Escape) return;

                if (keyInfo.Key == ConsoleKey.C)
                {
                    characterMenu.Show();
                    continue;
                }

                CurrentMap.MovePersons(keyInfo.Key);
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