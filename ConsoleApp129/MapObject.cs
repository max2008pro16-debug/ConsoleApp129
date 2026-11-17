using System;

namespace ConsoleApp129
{
    internal abstract class MapObject
    {
        public abstract char RenderOnMap();
    }

    internal class Wall : MapObject
    {
        public override char RenderOnMap() { Console.ForegroundColor = ConsoleColor.Cyan; return '+'; }
    }

    internal class Field : MapObject
    {
        public override char RenderOnMap() { Console.ForegroundColor = ConsoleColor.DarkGreen; return '.'; }
    }

    internal class Tree : MapObject
    {
        public override char RenderOnMap() { Console.ForegroundColor = ConsoleColor.Green; return 'T'; }
    }

    internal class Skull : MapObject
    {
        public override char RenderOnMap() { Console.ForegroundColor = ConsoleColor.White; return '☠'; }
    }

    internal class Portal : MapObject
    {
        public override char RenderOnMap() { Console.ForegroundColor = ConsoleColor.Magenta; return 'O'; }
    }

    internal class Artifact : MapObject
    {
        public int Type { get; set; }
        public Artifact(int type) { Type = type; }
        public override char RenderOnMap()
        {
            Console.ForegroundColor = ArtifactInfo.GetColor(Type);
            return ArtifactInfo.GetSymbol(Type);
        }
    }
}