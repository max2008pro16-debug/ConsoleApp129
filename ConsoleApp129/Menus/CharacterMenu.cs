using ConsoleApp129.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp129.Menus
{
    internal sealed class CharacterMenu : IMenu
    {
        private readonly GameService _gameService;
        private readonly CharacterInteractionService _interactionService;
        private readonly MenuManager _manager;

        public string Title { get { return "Меню взаимодействия с персонажами"; } }

        public CharacterMenu(GameService gameService, CharacterInteractionService interactionService)
        {
            _gameService = gameService;
            _interactionService = interactionService;

            _manager = new MenuManager(Title, new List<MenuItem>
            {
                new MenuItem("Показать всех персонажей", ShowCharacters),
                new MenuItem("Поговорить с персонажем", TalkWithCharacter),
                new MenuItem("Выбрать тип артефакта", SelectArtifactType),
                new MenuItem("Отдохнуть (+10 HP, ход врагов)", Rest)
            });
        }

        public void Show()
        {
            if (!_gameService.IsGameActive)
            {
                Console.WriteLine("Нет активной игры. Запустите новую игру.");
                return;
            }
            _manager.Run();
        }

        private void ShowCharacters()
        {
            var characters = _gameService.CurrentMap.GetAllPersons();
            if (characters.Count == 0) { Console.WriteLine("На карте нет персонажей."); return; }
            for (var i = 0; i < characters.Count; i++) Console.WriteLine("{0}. {1}", i + 1, characters[i]);
        }

        private void TalkWithCharacter()
        {
            var characters = _gameService.CurrentMap.GetAllPersons();
            if (characters.Count == 0) { Console.WriteLine("Некого слушать."); return; }
            ShowCharacters();
            Console.Write("Введите номер персонажа: ");
            int index;
            if (!int.TryParse(Console.ReadLine(), out index) || index < 1 || index > characters.Count) { Console.WriteLine("Некорректный номер."); return; }
            _interactionService.TalkTo(characters[index - 1]);
        }

        private void SelectArtifactType()
        {
            var hero = _gameService.CurrentMap.GetAllPersons().FirstOrDefault(p => p is Hero) as Hero;
            if (hero == null) return;

            Console.WriteLine("Ваши артефакты в инвентаре:");
            for (int i = 1; i <= 5; i++)
            {
                Console.ForegroundColor = ArtifactInfo.GetColor(i);
                Console.Write("  " + ArtifactInfo.GetSymbol(i));
                Console.ResetColor();
                Console.WriteLine(string.Format(" [{0}] {1} — {2} шт.  ({3})",
                    i, ArtifactInfo.GetName(i), hero.InventoryArtifacts[i], ArtifactInfo.GetDescription(i)));
            }
            Console.WriteLine();
            Console.WriteLine("Сейчас выбран: " + ArtifactInfo.GetName(hero.SelectedArtifactType));
            Console.Write("Введите номер печати (1-5): ");

            int choice;
            if (int.TryParse(Console.ReadLine(), out choice) && choice >= 1 && choice <= 5)
            {
                hero.SelectedArtifactType = choice;
                Console.WriteLine("Теперь клавишей '4' вы ставите: " + ArtifactInfo.GetName(choice));
            }
            else Console.WriteLine("Некорректный ввод.");
        }

        private void Rest()
        {
            var hero = _gameService.CurrentMap.GetAllPersons().FirstOrDefault(p => p is Hero) as Hero;
            if (hero == null || !hero.IsAlive) return;

            if (hero.Health >= hero.MaxHealth) { Console.WriteLine("У вас уже максимальное здоровье."); return; }

            hero.Heal(10);
            Console.WriteLine(string.Format("Вы отдохнули и восстановили 10 HP. Текущее HP: {0}/{1}", hero.Health, hero.MaxHealth));
            Console.WriteLine("Пока вы отдыхали, враги сделали ход...");
            _gameService.CurrentMap.MovePersons();
        }
    }
}