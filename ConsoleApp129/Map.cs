using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp129
{
    /// Класс карты: генерация (комнаты, леса, горы), отрисовка, движение.
    public class Map
    {
        private readonly Random _rand = new Random();
        private MapObject[,] _map;
        private int _width;
        private int _height;

        public int Width => _width;
        public int Height => _height;
        public MapObject[,] Cells => _map;

        public void Generate(int width = 25, int height = 25)
        {
            _width = width;
            _height = height;
            _map = new MapObject[width, height];

            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    _map[i, j] = new Field();

            GenerateRooms(min: 3, max: 5);
            GenerateForests(min: 3, max: 5);
            GenerateMountains(min: 2, max: 4);
            GenerateEnemies(min: 10, max: 15);

            int cx = _width / 2, cy = _height / 2;
            _map[cx, cy] = new Hero(cx, cy);
        }

        private void GenerateRooms(int min, int max)
        {
            int count = _rand.Next(min, max + 1);
            for (int k = 0; k < count; k++)
            {
                int roomW = _rand.Next(4, 9);
                int roomH = _rand.Next(4, 9);
                int startX = _rand.Next(1, _width - roomW - 1);
                int startY = _rand.Next(1, _height - roomH - 1);

                for (int i = startX; i < startX + roomW; i++)
                    for (int j = startY; j < startY + roomH; j++)
                        if (i == startX || i == startX + roomW - 1 ||
                            j == startY || j == startY + roomH - 1)
                            _map[i, j] = new Wall();

                switch (_rand.Next(4))
                {
                    case 0: _map[startX, startY + _rand.Next(1, roomH - 1)] = new Field(); break;
                    case 1: _map[startX + roomW - 1, startY + _rand.Next(1, roomH - 1)] = new Field(); break;
                    case 2: _map[startX + _rand.Next(1, roomW - 1), startY] = new Field(); break;
                    case 3: _map[startX + _rand.Next(1, roomW - 1), startY + roomH - 1] = new Field(); break;
                }
            }
        }

        private void GenerateForests(int min, int max)
        {
            int count = _rand.Next(min, max + 1);
            for (int k = 0; k < count; k++)
            {
                int cx = _rand.Next(2, _width - 2);
                int cy = _rand.Next(2, _height - 2);
                int radius = _rand.Next(1, 3);
                for (int dx = -radius; dx <= radius; dx++)
                    for (int dy = -radius; dy <= radius; dy++)
                    {
                        int x = cx + dx, y = cy + dy;
                        if (x > 0 && x < _width - 1 && y > 0 && y < _height - 1
                            && _rand.Next(100) < 70 && _map[x, y] is Field)
                        {
                            _map[x, y] = new Forest();
                        }
                    }
            }
        }

        private void GenerateMountains(int min, int max)
        {
            int count = _rand.Next(min, max + 1);
            for (int k = 0; k < count; k++)
            {
                int cx = _rand.Next(3, _width - 3);
                int cy = _rand.Next(3, _height - 3);
                int radius = _rand.Next(2, 4);
                for (int dx = -radius; dx <= radius; dx++)
                    for (int dy = -radius; dy <= radius; dy++)
                    {
                        int x = cx + dx, y = cy + dy;
                        if (x > 0 && x < _width - 1 && y > 0 && y < _height - 1
                            && Math.Abs(dx) + Math.Abs(dy) <= radius
                            && _map[x, y] is Field)
                            _map[x, y] = new Mountain();
                    }
            }
        }

        private void GenerateEnemies(int min, int max)
        {
            int count = _rand.Next(min, max + 1);
            int placed = 0, attempts = 0;
            while (placed < count && attempts < 1000)
            {
                attempts++;
                int x = _rand.Next(_width);
                int y = _rand.Next(_height);
                if (_map[x, y] is Field)
                {
                    _map[x, y] = new Enemy(x, y);
                    placed++;
                }
            }
        }

        public void Draw()
        {
            for (int i = 0; i < _width; i++)
            {
                for (int j = 0; j < _height; j++)
                {
                    var (sym, col) = _map[i, j].Render();
                    Console.ForegroundColor = col;
                    Console.Write(sym + " ");
                    Console.ResetColor();
                }
                Console.WriteLine();
            }
        }

        public void MoveEnemies()
        {
            var newMap = (MapObject[,])_map.Clone();

            for (int i = 0; i < _width; i++)
                for (int j = 0; j < _height; j++)
                {
                    if (_map[i, j] is Enemy)
                    {
                        int dir = _rand.Next(4);
                        int nx = i, ny = j;
                        switch (dir)
                        {
                            case 0: nx = (i - 1 + _width) % _width; break;
                            case 1: nx = (i + 1) % _width; break;
                            case 2: ny = (j - 1 + _height) % _height; break;
                            case 3: ny = (j + 1) % _height; break;
                        }
                        if (newMap[nx, ny] is Field)
                        {
                            newMap[nx, ny] = _map[i, j];
                            newMap[i, j] = new Field();
                        }
                    }
                }
            _map = newMap;
        }

        public void MoveHero(ConsoleKey key)
        {
            var newMap = (MapObject[,])_map.Clone();

            for (int i = 0; i < _width; i++)
                for (int j = 0; j < _height; j++)
                {
                    if (_map[i, j] is Hero)
                    {
                        int nx = i, ny = j;
                        switch (key)
                        {
                            case ConsoleKey.UpArrow: nx = (i - 1 + _width) % _width; break;
                            case ConsoleKey.DownArrow: nx = (i + 1) % _width; break;
                            case ConsoleKey.LeftArrow: ny = (j - 1 + _height) % _height; break;
                            case ConsoleKey.RightArrow: ny = (j + 1) % _height; break;
                        }
                        if (newMap[nx, ny] is Field)
                        {
                            newMap[nx, ny] = _map[i, j];
                            newMap[i, j] = new Field();
                        }
                    }
                }
            _map = newMap;
        }

        public Hero FindHero()
        {
            foreach (var obj in _map)
                if (obj is Hero h) return h;
            return null;
        }
    }
}
