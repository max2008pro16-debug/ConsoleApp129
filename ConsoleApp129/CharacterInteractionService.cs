using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ConsoleApp129.Services
{
    internal sealed class CharacterInteractionService
    {
        public void TalkTo(Person character)
        {
            if (character == null) throw new ArgumentNullException(nameof(character));

            if (character is Hero)
                Console.WriteLine(character.Name + ": «Я готов к приключениям!»");
            else
                Console.WriteLine(character.Name + ": «Убирайся с моей земли, путник!»");
        }

        public void Attack(Person target, int damage)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));

            target.TakeDamage(damage);
            Console.WriteLine(string.Format("{0} получает {1} урона. Текущее здоровье: {2}/{3}.",
                target.Name, damage, target.Health, target.MaxHealth));
        }

        public void Heal(Person character, int amount)
        {
            if (character == null) throw new ArgumentNullException(nameof(character));

            character.Heal(amount);
            Console.WriteLine(string.Format("{0} восстановил {1} здоровья. Текущее здоровье: {2}/{3}.",
                character.Name, amount, character.Health, character.MaxHealth));
        }
    }
}