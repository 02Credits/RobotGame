using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using RobotGameShared.Renderer;
using RobotGameShared.SpriteSheet;

namespace RobotGameShared.Map {
    public class WorldRenderer : IDrawable {
        private readonly MapManager mapManager;
        private readonly SheetManager sheetManager;

        public float TileWidth => 1;
        public float TileHeight => TileWidth / 2;

        public Vector2? InputWorldPosition { get; private set; }

        public float DrawOrder => (float)DrawStages.World;

        public WorldRenderer(MapManager mapManager, SheetManager sheetManager) {
            this.mapManager = mapManager;
            this.sheetManager = sheetManager;
        }

        public void Draw(GameTime gameTime) {
            InputWorldPosition = null;
            for (int x = 0; x < mapManager.TileMap.Length; x++) {
                var mapColumn = mapManager.TileMap[x];
                var entityColumn = mapManager.EntityMap[x];
                for (int y = 0; y < mapColumn.Length; y++) {
                    (Tiles tile, int height) = mapColumn[y];
                    Vector3 position = new Vector3(x, y, height);
                    var mousePosition = new Vector2(x, y);
                    DrawTile(tile, position, hover: () => InputWorldPosition = mousePosition);
                    Tiles? entityTile = entityColumn[y];
                    if (entityTile != null) {
                        DrawTile(entityTile.Value, position, 2);
                    }
                }
            }
        }

        private void DrawTile(Tiles tileToDraw, Vector3 worldPosition, float layerOffset = 0, Action click = null, Action hover = null) {
            Vector2 screenPosition = WorldToScreen(worldPosition);
            float layerDepth = worldPosition.Y - worldPosition.X + layerOffset;
            sheetManager.DrawTile(tileToDraw, screenPosition, TileWidth, layerDepth, click, hover);
        }

        public void DrawEntity(Tiles entityTile, Vector2 mapPosition, Action click = null, Action hover = null) {
            (Tiles _, int height) = mapManager.TileMap.ElementAt(mapPosition).Value;
            DrawTile(entityTile, new Vector3(mapPosition, height), 2, click, hover);
        }

        public Vector2 WorldToScreen(Vector3 worldPosition) {
            return new Vector2(
                worldPosition.X * TileWidth + worldPosition.Y * TileWidth,
                -worldPosition.X * TileHeight + worldPosition.Y * TileHeight - worldPosition.Z * TileHeight) / 2;
        }
    }
}
