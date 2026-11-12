using ConsoleApp129.Menus;
using ConsoleApp129.SaveSystem;
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
        private const string JsonPath = "savegame.json";
        private const string XmlPath = "savegame.xml";

        private readonly MenuManager _manager;
        private readonly GameService _gameService;
        private readonly CharacterMenu _characterMenu;
        private readonly ISaveService _jsonService;
        private readonly ISaveService _xmlService;

        public string Title { get { return "Главное меню игры"; } }

        public MainMenu(GameService gameService, CharacterMenu characterMenu)
        {
            _gameService = gameService;
            _characterMenu = characterMenu;
            _jsonService = SaveServiceFactory.Create(SaveFormat.Json);
            _xmlService = SaveServiceFactory.Create(SaveFormat.Xml);

            _manager = new MenuManager(Title, new[]
            {
                new MenuItem("Новая игра", StartNewGame),
                new MenuItem("Загрузить игру (JSON)", LoadJson),
                new MenuItem("Загрузить игру (XML)", LoadXml),
                new MenuItem("Меню персонажей", ShowCharacterMenu),
                new MenuItem("Сохранить игру (JSON)", SaveJson),
                new MenuItem("Сохранить игру (XML)", SaveXml),
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

        private void SaveJson() { SaveGame(_jsonService, JsonPath); }
        private void SaveXml() { SaveGame(_xmlService, XmlPath); }
        private void LoadJson() { LoadGame(_jsonService, JsonPath); }
        private void LoadXml() { LoadGame(_xmlService, XmlPath); }

        private void SaveGame(ISaveService saveService, string path)
        {
            if (!_gameService.IsGameActive) { Console.WriteLine("Нет игры для сохранения."); return; }
            var data = _gameService.CurrentMap.ToSaveData();
            saveService.Save(data, path);
            Console.WriteLine("Игра сохранена: " + Path.GetFullPath(path));
        }

        private void LoadGame(ISaveService saveService, string path)
        {
            var data = saveService.Load(path);
            var map = new Map();
            map.LoadFromSaveData(data);
            _gameService.SetMap(map);
            Console.WriteLine("Игра загружена: " + Path.GetFullPath(path));
            _gameService.RunGameLoop(_characterMenu);
        }

        private static void ShowAbout()
        {
            Console.WriteLine("Учебный проект: меню игры, персонажи и сохранения JSON/XML.");
            Console.WriteLine("Практическая работа №3, МДК.03.01.");
        }
    }
}