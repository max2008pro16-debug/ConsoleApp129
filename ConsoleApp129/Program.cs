using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp129.Menus;
using ConsoleApp129.Services;

namespace ConsoleApp129
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.CursorVisible = false;

            var gameService = new GameService();
            var interactionService = new CharacterInteractionService();
            var characterMenu = new CharacterMenu(gameService, interactionService);
            var mainMenu = new MainMenu(gameService, characterMenu);

            mainMenu.Show();

            Console.CursorVisible = true;
        }
    }
}