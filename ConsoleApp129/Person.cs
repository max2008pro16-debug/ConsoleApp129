using System;

namespace ConsoleApp129
{
    /// Базовый класс для всех персонажей на карте (Герой, Враг, Союзник).
    internal class Person : MapObject
    {
        ///  Имя персонажа.
        public string Name { get; set; }

        ///  Максимальный запас здоровья.
        public int MaxHealth { get; internal set; }

        ///  Текущий запас здоровья.
        public int Health { get; internal set; }

        ///  Сила атаки персонажа. 
        public int Damage { get; internal set; }

        /// Проверяет, жив ли персонаж.
        public bool IsAlive { get { return Health > 0; } }

        /// Инициализирует новый экземпляр класса 
        ///Координата X.
        ///Координата Y.
        ///Имя персонажа.
        ///Максимальное здоровье.
        ///Урон.
        protected Person(int x, int y, string name, int maxHealth, int damage)
        {
            Name = name;
            MaxHealth = maxHealth;
            Health = maxHealth;
            Damage = damage;
        }

        /// Наносит персонажу урон.
        ///Количество урона.
        public void TakeDamage(int dmg)
        {
            Health = Math.Max(0, Health - dmg);
        }

        /// Восстанавливает здоровье персонажа.
        ///Количество восстанавливаемого здоровья.
        public void Heal(int amount)
        {
            Health = Math.Min(MaxHealth, Health + amount);
        }

        /// Возвращает символ для отрисовки персонажа на карте.
        public override char RenderOnMap()
        {
            return '☺';
        }

        /// Возвращает строковое представление персонажа с его характеристиками.
        public override string ToString()
        {
            return string.Format("{0} (HP: {1}/{2}, Урон: {3})", Name, Health, MaxHealth, Damage);
        }
    }

    /// Класс, описывающий игрока.
    internal class Hero : Person
    {
        public int InventoryWalls { get; set; }
        public int InventoryTrees { get; set; }
        public int InventorySkulls { get; set; }
        public int Level { get; set; } = 1;
        public int Coins { get; set; } = 0;
        public int BonusAllyHealth { get; set; } = 0;
        public int BonusAllyDamage { get; set; } = 0;

        /// Инициализирует нового Героя.
        public Hero(int x, int y) : base(x, y, "Герой", 100, 20)
        {
            InventoryWalls = 0;
            InventoryTrees = 0;
            InventorySkulls = 0;
        }

        public override char RenderOnMap()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            return '☻';
        }
    }

    /// Класс врага.
    internal class Enemy : Person
    {
        private static int _counter;

        /// Инициализирует нового Врага.
        public Enemy(int x, int y) : base(x, y, string.Format("Враг #{0}", ++_counter), 50, 10) { }

        public override char RenderOnMap()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            return base.RenderOnMap();
        }
    }

    /// Класс Союзника, призываемого игроком.
    internal class Ally : Person
    {
        /// Инициализирует нового Союзника с бонусами от Героя.
        public Ally(int x, int y, int heroLevel, int bonusHp, int bonusDmg)
            : base(x, y, "Союзник", 80 + (heroLevel * 10) + bonusHp, 15 + (heroLevel * 2) + bonusDmg) { }

        public override char RenderOnMap()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            return 'A';
        }
    }
}