using System;
using System.Collections.Generic;
using ConsoleApp129.Services;

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
                new MenuItem("Нанести урон персонажу", DamageCharacter),
                new MenuItem("Вылечить персонажа", HealCharacter)
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
            Person character = SelectCharacter();
            if (character != null) _interactionService.TalkTo(character);
        }

        private void DamageCharacter()
        {
            Person character = SelectCharacter();
            if (character == null) return;

            var damage = ReadNonNegativeInt("Введите урон: ");
            _interactionService.Attack(character, damage);

            if (_gameService.CurrentMap != null) _gameService.CurrentMap.CleanupDeadCharacters();
        }

        private void HealCharacter()
        {
            Person character = SelectCharacter();
            if (character == null) return;

            var amount = ReadNonNegativeInt("Введите количество лечения: ");
            _interactionService.Heal(character, amount);
        }

        private Person SelectCharacter()
        {
            var characters = GetPersons();
            if (characters.Count == 0)
            {
                Console.WriteLine("Нет доступных персонажей.");
                return null;
            }

            ShowCharacters();
            Console.Write("Введите номер персонажа: ");
            int index;

            if (!int.TryParse(Console.ReadLine(), out index) || index < 1 || index > characters.Count)
            {
                Console.WriteLine("Некорректный номер персонажа.");
                return null;
            }

            return characters[index - 1];
        }

        private static int ReadNonNegativeInt(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                int value;
                if (int.TryParse(Console.ReadLine(), out value) && value >= 0) return value;
                Console.WriteLine("Введите неотрицательное целое число.");
            }
        }
    }
}