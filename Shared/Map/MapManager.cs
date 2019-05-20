using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Input;
using RobotGameShared.SpriteSheet;

namespace RobotGameShared.Map {
    public class MapManager : IUpdateable {
        private Dictionary<(int left, int up, int right, int down), Tiles> SlopeLookup = new Dictionary<(int, int, int, int), Tiles> {
            [(0, 0, 0, 0)] = Tiles.Flat,
            [(1, 1, 0, 0)] = Tiles.SlopeLeft,
            [(1, 1, 1, 0)] = Tiles.SlopeDown,
            [(0, 1, 1, 0)] = Tiles.SlopeRight,
            [(0, 1, 0, 0)] = Tiles.SlopeUp
        };

        public (Tiles tile, int height)[][] TileMap;
        public Tiles?[][] EntityMap;

        public int Radius = 10;

        public int Width => Radius;
        public int Height => Radius;

        public int UpdateOrder => 0;

        public Vector2 BallPosition { get; private set; }

        private Random random;

        public MapManager(Random random) {
            this.random = random;
            RandomizeMap();
        }

        public void Update(GameTime gameTime) {
#if IOS
            TouchCollection tc = TouchPanel.GetState();
            if (tc.Any()) {
                RandomizeMap();
            }
#else
            var mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed) {
                RandomizeMap();
            }
#endif
        }

        public void RandomizeMap() {
            int[][] heightMap = new int[Radius + 1][];
            for (int x = Radius; x >= 0; x--) {
                heightMap[x] = new int[Radius + 1];
                for (int y = 0; y < Radius + 1; y++) {
                    if (y == 0 || x == Radius) heightMap[x][y] = 0;
                    else {
                        int upParent = heightMap[x + 1][y - 1];
                        int leftParent = heightMap[x][y - 1];
                        int rightParent = heightMap[x + 1][y];

                        if (leftParent == upParent && rightParent == upParent && random.Next(30) == 1) {
                            heightMap[x][y] = leftParent - 1;
                        } else {
                            heightMap[x][y] = new[] { leftParent, upParent, rightParent }.Min();
                        }
                    }
                }
            }

            TileMap = new (Tiles, int)[Radius][];
            EntityMap = new Tiles?[Radius][];
            for (int x = 0; x < TileMap.Length; x++) {
                TileMap[x] = new (Tiles, int)[Radius];
                EntityMap[x] = new Tiles?[Radius];
                for (int y = 0; y < TileMap[x].Length; y++) {
                    int left = heightMap[x][y];
                    int up = heightMap[x + 1][y];
                    int right = heightMap[x + 1][y + 1];
                    int down = heightMap[x][y + 1];

                    int lowest = new[] { left, up, right, down }.Min();
                    Tiles tile = SlopeLookup[(left - lowest, up - lowest, right - lowest, down - lowest)];
                    TileMap[x][y] = (tile, lowest);
                }
            }

            for (int i = 0; i < 10; i++) {
                int x = random.Next(Width - 1);
                int y = random.Next(Height - 1);

                if (TileMap[x][y].tile == Tiles.Flat && EntityMap[x][y] == null) {
                    Tiles tile = random.Next(2) == 1 ? Tiles.Tree : Tiles.Wall;
                    EntityMap[x][y] = tile;
                }
            }

            while (true) {
                int x = random.Next(Width - 1);
                int y = random.Next(Height - 1);
                if (TileMap[x][y].tile == Tiles.Flat && EntityMap[x][y] == null) {
                    EntityMap[x][y] = Tiles.Ball;
                    BallPosition = new Vector2(x, y);
                    break;
                }
            }
        }

        public Tiles? TileAt(Vector2 position) {
            int xIndex = (int)position.X;
            int yIndex = (int)position.Y;
            if (xIndex >= 0 && xIndex < Radius &&
                yIndex >= 0 && yIndex < Radius) {
                return TileMap[xIndex][yIndex].tile;
            }

            return null;
        }

        public Tiles? EntityAt(Vector2 position) {
            int xIndex = (int)position.X;
            int yIndex = (int)position.Y;
            if (xIndex >= 0 && xIndex < Radius &&
                yIndex >= 0 && yIndex < Radius) {
                return EntityMap[xIndex][yIndex];
            }

            return null;
        }
    }
}
