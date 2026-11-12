using ConsoleApp129.Menus;
using ConsoleApp129.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp129.Menus
{
    internal sealed class MainMenu : IMenu
    {
        private readonly MenuManager _manager;
        private readonly GameService _gameService;
        private readonly CharacterMenu _characterMenu;

        public string Title { get { return "Главное меню игры"; } }

        public MainMenu(GameService gameService, CharacterMenu characterMenu)
        {
            if (gameService == null) throw new ArgumentNullException(nameof(gameService));
            if (characterMenu == null) throw new ArgumentNullException(nameof(characterMenu));

            _gameService = gameService;
            _characterMenu = characterMenu;

            _manager = new MenuManager(Title, new[]
            {
                new MenuItem("Новая игра", StartNewGame),
                new MenuItem("Меню персонажей", ShowCharacterMenu),
                new MenuItem("О программе", ShowAbout)
            });
        }

        public void Show() { _manager.Run(); }

        private void StartNewGame()
        {
            _gameService.StartNewGame();
            _gameService.RunGameLoop(_characterMenu);
        }

        private void ShowCharacterMenu()
        {
            if (!_gameService.IsGameActive)
            {
                Console.WriteLine("Нет активной игры. Сначала запустите новую игру.");
                return;
            }
            _characterMenu.Show();
        }

        private static void ShowAbout()
        {
            Console.WriteLine("Учебный проект: меню игры и взаимодействие с персонажами.");
            Console.WriteLine("Практическая работа №2, МДК.02.01.");
        }
    }
}