using System;

namespace ConsoleApp129
{
    internal enum LevelType
    {
        Normal,
        ChaosBoss,   // 1 - Пламя → Владыка Пламени
        OrderBoss,   // 2 - Лёд → Ледяной Страж
        Arena,       // 3 - Гром → Зал Испытаний
        Abyss,       // 4 - Тьма → Бездна
        Sanctuary    // 5 - Свет → Святилище
    }

    internal static class ArtifactInfo
    {
        public static string GetName(int type)
        {
            switch (type)
            {
                case 1: return "Пламенная Печать";
                case 2: return "Ледяная Печать";
                case 3: return "Громовая Печать";
                case 4: return "Тёмная Печать";
                case 5: return "Светлая Печать";
                default: return "Неизвестная Печать";
            }
        }

        public static char GetSymbol(int type)
        {
            switch (type)
            {
                case 1: return '♦';
                case 2: return '❄';
                case 3: return '§';
                case 4: return '☾';
                case 5: return '☼';
                default: return '?';
            }
        }

        public static ConsoleColor GetColor(int type)
        {
            switch (type)
            {
                case 1: return ConsoleColor.Red;
                case 2: return ConsoleColor.Cyan;
                case 3: return ConsoleColor.Yellow;
                case 4: return ConsoleColor.DarkMagenta;
                case 5: return ConsoleColor.White;
                default: return ConsoleColor.Gray;
            }
        }

        public static LevelType GetLevelType(int type)
        {
            switch (type)
            {
                case 1: return LevelType.ChaosBoss;
                case 2: return LevelType.OrderBoss;
                case 3: return LevelType.Arena;
                case 4: return LevelType.Abyss;
                case 5: return LevelType.Sanctuary;
                default: return LevelType.Normal;
            }
        }

        public static string GetDescription(int type)
        {
            switch (type)
            {
                case 1: return "Огонь и Хаос. Ведёт к Владыке Пламени.";
                case 2: return "Лёд и Порядок. Ведёт к Ледяному Стражу.";
                case 3: return "Гром и Сила. Ведёт в Зал Испытаний.";
                case 4: return "Тьма и Смерть. Ведёт в Бездну.";
                case 5: return "Свет и Жизнь. Ведёт в Святилище.";
                default: return "Неизвестная сила.";
            }
        }
    }
}