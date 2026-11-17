using System;

namespace ConsoleApp129
{
    internal class Person : MapObject
    {
        public string Name { get; set; }
        public int MaxHealth { get; internal set; }
        public int Health { get; internal set; }
        public int Damage { get; internal set; }
        public bool IsAlive { get { return Health > 0; } }

        protected Person(int x, int y, string name, int maxHealth, int damage)
        {
            Name = name;
            MaxHealth = maxHealth;
            Health = maxHealth;
            Damage = damage;
        }

        public void TakeDamage(int dmg) { Health = Math.Max(0, Health - dmg); }
        public void Heal(int amount) { Health = Math.Min(MaxHealth, Health + amount); }

        public override char RenderOnMap() { return '☺'; }

        // БАЗОВЫЙ ВЫВОД (для врагов и союзников) - С УРОНОМ
        public override string ToString()
        {
            return string.Format("{0} (HP: {1}/{2}, Урон: {3})", Name, Health, MaxHealth, Damage);
        }
    }

    internal class Hero : Person
    {
        public int InventoryWalls { get; set; }
        public int InventoryTrees { get; set; }
        public int InventorySkulls { get; set; }

        public int[] InventoryArtifacts { get; set; }
        public bool[] BossDefeated { get; set; }

        public int SelectedArtifactType { get; set; } = 1;

        public int Level { get; set; } = 1;
        public int Coins { get; set; } = 0;
        public int BonusAllyHealth { get; set; } = 0;
        public int BonusAllyDamage { get; set; } = 0;

        public Hero(int x, int y) : base(x, y, "Герой", 100, 0) // Урон 0
        {
            InventoryArtifacts = new int[6];
            BossDefeated = new bool[6];
        }

        public override char RenderOnMap() { Console.ForegroundColor = ConsoleColor.Yellow; return '☻'; }

        public override string ToString()
        {
            return string.Format("{0} (HP: {1}/{2})", Name, Health, MaxHealth);
        }
    }

    internal class Enemy : Person
    {
        private static int _counter;
        public Enemy(int x, int y) : base(x, y, string.Format("Враг #{0}", ++_counter), 50, 10) { }
        public override char RenderOnMap() { Console.ForegroundColor = ConsoleColor.Red; return base.RenderOnMap(); }
    }

    internal class Hunter : Person
    {
        public Hunter(int x, int y) : base(x, y, "Охотник", 40, 15) { }
        public override char RenderOnMap() { Console.ForegroundColor = ConsoleColor.DarkRed; return 'h'; }
    }

    internal class Archer : Person
    {
        public Archer(int x, int y) : base(x, y, "Лучник", 60, 10) { }
        public override char RenderOnMap() { Console.ForegroundColor = ConsoleColor.Magenta; return 'a'; }
    }

    internal class Ally : Person
    {
        public Ally(int x, int y, int heroLevel, int bonusHp, int bonusDmg)
            : base(x, y, "Союзник", 80 + (heroLevel * 10) + bonusHp, 15 + (heroLevel * 2) + bonusDmg) { }
        public override char RenderOnMap() { Console.ForegroundColor = ConsoleColor.Cyan; return 'A'; }
    }

    internal class Boss : Person
    {
        public int BossType { get; set; }
        public Boss(int x, int y, int type) : base(x, y, "Босс " + type, 250 + (type * 50), 20 + (type * 5))
        {
            BossType = type;
        }
        public override char RenderOnMap() { Console.ForegroundColor = ConsoleColor.Red; return 'B'; }
    }
}