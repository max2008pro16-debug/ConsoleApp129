using System;

namespace ConsoleApp129
{
    internal class Person : MapObject
    {
        private readonly int pointX;
        private readonly int pointY;

        public string Name { get; set; }
        public int MaxHealth { get; }
        public int Health { get; internal set; }

        public bool IsAlive { get { return Health > 0; } }

        protected Person(int x, int y, string name, int maxHealth, int? currentHealth = null)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Имя пустое.", nameof(name));
            if (maxHealth <= 0) throw new ArgumentOutOfRangeException(nameof(maxHealth));

            pointX = x;
            pointY = y;
            Name = name;
            MaxHealth = maxHealth;

            int hp = currentHealth ?? maxHealth;
            Health = Math.Min(Math.Max(hp, 0), maxHealth);
        }

        public void TakeDamage(int damage)
        {
            if (damage < 0) throw new ArgumentOutOfRangeException(nameof(damage));
            Health = Math.Max(0, Health - damage);
        }

        public void Heal(int amount)
        {
            if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount));
            Health = Math.Min(MaxHealth, Health + amount);
        }

        public override char Rendering_on_the_map()
        {
            return '☺';
        }

        public override string ToString()
        {
            return string.Format("{0} (HP: {1}/{2})", Name, Health, MaxHealth);
        }
    }

    internal class Hero : Person
    {
        // ИНВЕНТАРЬ ГЕРОЯ
        public int InventoryWalls { get; set; }
        public int InventoryTrees { get; set; }

        public Hero(int X, int Y) : base(X, Y, "Герой", 100)
        {
            InventoryWalls = 0;
            InventoryTrees = 0;
        }

        public override char Rendering_on_the_map()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            return '☻';
        }
    }

    internal class Enemy : Person
    {
        private static int _counter;

        public Enemy(int X, int Y) : base(X, Y, string.Format("Враг #{0}", ++_counter), 50) { }

        public override char Rendering_on_the_map()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            return base.Rendering_on_the_map();
        }
    }

    // НОВЫЙ КЛАСС: СОЮЗНИК
    internal class Ally : Person
    {
        public Ally(int X, int Y) : base(X, Y, "Союзник", 80) { }

        public override char Rendering_on_the_map()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            return 'A';
        }
    }
}