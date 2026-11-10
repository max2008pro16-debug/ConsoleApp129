using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp129
{
    /// DTO для сохранения состояния игры в JSON.
    /// Содержит только данные (никакой логики) — это позволяет
    /// использовать System.Text.Json без циклических ссылок.
    public class SaveData
    {
            public int Width { get; set; }
            public int Height { get; set; }
            public int HeroX { get; set; }
            public int HeroY { get; set; }
            public int HeroHealth { get; set; }
            public int HeroScore { get; set; }
            public List<ObjectDto> Objects { get; set; } = new List<ObjectDto>();

        /// <summary>Один объект карты в сериализованном виде.</summary>
        public class ObjectDto
        {
            /// <summary>Тип: Field, Wall, Tree, Forest, Mountain, Enemy.</summary>
            public string Type { get; set; }
            public int X { get; set; }
            public int Y { get; set; }
        }
    }
}
