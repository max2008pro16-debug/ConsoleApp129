using ConsoleApp129.SaveSystem;
using ConsoleApp129.Services;
using System;
using System.IO;
namespace ConsoleApp129.Menus
{
    internal sealed class MainMenu : IMenu
    {
        private const string SavePath = "savegame.json";

        private readonly MenuManager _manager;
        private readonly GameService _gameService;
        private readonly CharacterMenu _characterMenu;
        private readonly ISaveService _saveService;

        public string Title { get { return "Главное меню игры"; } }

        public MainMenu(GameService gameService, CharacterMenu characterMenu)
        {
            _gameService = gameService;
            _characterMenu = characterMenu;

            _saveService = SaveServiceFactory.Create(SaveFormat.Json);

            _manager = new MenuManager(Title, new[]
            {
                new MenuItem("Новая игра", StartNewGame),
                new MenuItem("Загрузить игру", LoadGame),
                new MenuItem("Меню персонажей", ShowCharacterMenu),
                new MenuItem("Сохранить игру", SaveGame),
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
            if (_gameService.IsGameActive) _characterMenu.Show();
            else Console.WriteLine("Нет активной игры.");
        }

        private void SaveGame()
        {
            if (!_gameService.IsGameActive)
            {
                Console.WriteLine("Нет игры для сохранения.");
                return;
            }

            var data = _gameService.CurrentMap.ToSaveData();
            _saveService.Save(data, SavePath);
            Console.WriteLine("Игра сохранена: " + Path.GetFullPath(SavePath));
        }

        private void LoadGame()
        {
            var data = _saveService.Load(SavePath);
            var map = new Map();
            map.LoadFromSaveData(data);
            _gameService.SetMap(map);

            Console.WriteLine("Игра загружена: " + Path.GetFullPath(SavePath));
            _gameService.RunGameLoop(_characterMenu);
        }

        private static void ShowAbout()
        {
            Console.WriteLine("Учебный проект: меню игры, персонажи и сохранения JSON.");
            Console.WriteLine("Практическая работа №3, МДК.03.01.");
        }
    }
}