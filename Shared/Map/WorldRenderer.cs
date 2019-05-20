using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using RobotGameShared.SpriteSheet;

namespace RobotGameShared.Map {
    public class WorldRenderer : IDrawable {
        private readonly MapManager mapManager;
        private readonly ScreenSizeManager screenSizeManager;
        private readonly SheetManager sheetManager;

        public Vector2 CameraWorldPosition => new Vector2((float)mapManager.Radius / 2, (float)mapManager.Radius / 2);

        public float TileWidth => 1;
        public float TileHeight => TileWidth / 2;

        public int DrawOrder => 1;

        public WorldRenderer(MapManager mapManager, ScreenSizeManager screenSizeManager, SheetManager sheetManager) {
            this.mapManager = mapManager;
            this.screenSizeManager = screenSizeManager;
            this.sheetManager = sheetManager;
        }

        public void Draw(GameTime gameTime) {
            for (int x = 0; x < mapManager.TileMap.Length; x++) {
                var mapColumn = mapManager.TileMap[x];
                var entityColumn = mapManager.EntityMap[x];
                for (int y = 0; y < mapColumn.Length; y++) {
                    (Tiles tile, int height) = mapColumn[y];
                    Vector3 position = new Vector3(x, y, height);
                    DrawTile(tile, position);
                    Tiles? entityTile = entityColumn[y];
                    if (entityTile != null) {
                        DrawTile(entityTile.Value, position, 2);
                    }
                }
            }
        }

        private void DrawTile(Tiles tileToDraw, Vector3 worldPosition, float layerOffset = 0) {
            Vector2 screenPosition = WorldToScreen(worldPosition);
            float layerDepth = (worldPosition.Y - worldPosition.X + worldPosition.Z * 0.5f + layerOffset + 50.0f) / 100.0f;
            sheetManager.DrawTile(tileToDraw, screenPosition, TileWidth, Color.White, layerDepth);
        }

        public void DrawEntity(Tiles entityTile, Vector2 mapPosition) {
            (Tiles _, int height) = mapManager.TileMap.ElementAt(mapPosition).Value;
            DrawTile(entityTile, new Vector3(mapPosition, height), 2);
        }

        public Vector2 WorldToScreen(Vector3 worldPosition) {
            Vector3 relativePosition = worldPosition - new Vector3(CameraWorldPosition, 0) + new Vector3(0.5f, 0.5f, 0);

            return (new Vector2(
                relativePosition.X * TileWidth + relativePosition.Y * TileWidth,
                -relativePosition.X * TileHeight + relativePosition.Y * TileHeight - relativePosition.Z * TileHeight)
                + new Vector2(screenSizeManager.PixelWidth, screenSizeManager.PixelHeight)) / 2;
        }

        public Vector2 ScreenToWorld(Vector2 screenPosition) {
            Vector2 centeredScreenPosition = screenPosition * 2 - new Vector2(screenSizeManager.PixelWidth, screenSizeManager.PixelHeight * 2);
            Vector2 relativePosition = new Vector2(
                (centeredScreenPosition.X - centeredScreenPosition.Y) / (2 * TileWidth),
                (centeredScreenPosition.X + centeredScreenPosition.Y) / (2 * TileWidth));

            return relativePosition + CameraWorldPosition - new Vector2(0.5f, 0.5f);
        }
    }
}
