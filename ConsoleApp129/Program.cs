using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp129
{
    internal class Program
    {
        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.CursorVisible = false;

            while (true)
            {
                var mainMenu = new MainMenu();
                var choice = mainMenu.GetChoice();

                if (choice == MainMenu.Action.Exit) break;

                if (choice == MainMenu.Action.NewGame)
                {
                    var map = new Map();
                    map.Generate();
                    GameLoop(map);
                }
                else if (choice == MainMenu.Action.Load)
                {
                    try
                    {
                        // var map = SaveManager.Load();
                        // GameLoop(map);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка загрузки: {ex.Message}");
                        Console.ReadKey(true);
                    }
                }
            }

            Console.CursorVisible = true;
        }

        private static void GameLoop(Map map)
        {
            while (true)
            {
                Console.Clear();
                map.Draw();
                Console.WriteLine("\nСтрелки — движение, Esc — меню, C — персонаж");

                var key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.Escape)
                {
                    var gm = new GameMenu();
                    var act = gm.GetChoice();

                    if (act == GameMenu.Action.Continue) continue;
                    if (act == GameMenu.Action.ToMain) return;

                    if (act == GameMenu.Action.Character)
                    {
                        var h = map.FindHero();
                        // if (h != null) new CharacterMenu(h).Run();
                        continue;
                    }
                    if (act == GameMenu.Action.Save)
                    {
                        // SaveManager.Save(map);
                        continue;
                    }
                    if (act == GameMenu.Action.Load)
                    {
                        // map = SaveManager.Load();
                        continue;
                    }
                    continue;
                }

                if (key == ConsoleKey.C)
                {
                    var h = map.FindHero();
                    // if (h != null) new CharacterMenu(h).Run();
                    continue;
                }

                map.MoveHero(key);
                map.MoveEnemies();
            }
        }
    }
}