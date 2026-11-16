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
            if (gameService == null) throw new ArgumentNullException(nameof(gameService));
            if (interactionService == null) throw new ArgumentNullException(nameof(interactionService));

            _gameService = gameService;
            _interactionService = interactionService;

            _manager = new MenuManager(Title, new List<MenuItem>
            {
                new MenuItem("Показать всех персонажей", ShowCharacters),
                new MenuItem("Поговорить с персонажем", TalkWithCharacter),
                new MenuItem("Отдохнуть (+10 HP, ход врагов)", Rest) // ЗАМЕНА
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

        private IReadOnlyList<Person> GetPersons()
        {
            if (_gameService.CurrentMap != null)
                return _gameService.CurrentMap.GetAllPersons();
            return new List<Person>();
        }

        private void ShowCharacters()
        {
            var characters = GetPersons();
            if (characters.Count == 0)
            {
                Console.WriteLine("На карте нет персонажей.");
                return;
            }

            for (var i = 0; i < characters.Count; i++)
            {
                Console.WriteLine("{0}. {1}", i + 1, characters[i]);
            }
        }

        private void TalkWithCharacter()
        {
            var characters = GetPersons();
            if (characters.Count == 0)
            {
                Console.WriteLine("Некого слушать.");
                return;
            }

            ShowCharacters();
            Console.Write("Введите номер персонажа: ");
            int index;

            if (!int.TryParse(Console.ReadLine(), out index) || index < 1 || index > characters.Count)
            {
                Console.WriteLine("Некорректный номер.");
                return;
            }

            _interactionService.TalkTo(characters[index - 1]);
        }

        // НОВЫЙ МЕТОД ОТДЫХА
        private void Rest()
        {
            var hero = _gameService.CurrentMap.GetAllPersons().FirstOrDefault(p => p is Hero) as Hero;
            if (hero == null || !hero.IsAlive) return;

            if (hero.Health >= hero.MaxHealth)
            {
                Console.WriteLine("У вас уже максимальное здоровье.");
                return;
            }

            hero.Heal(10);
            Console.WriteLine(string.Format("Вы отдохнули и восстановили 10 HP. Текущее HP: {0}/{1}", hero.Health, hero.MaxHealth));
            Console.WriteLine("Пока вы отдыхали, враги сделали ход...");

            // Враги и союзники ходят, пока вы в меню
            _gameService.CurrentMap.MovePersons();
        }
    }
}