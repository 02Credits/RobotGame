using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using RobotGameShared.Map;
using RobotGameShared.Renderer;

namespace RobotGameShared.SpriteSheet {

    public class SheetManager : IContentLoadable {
        public Texture2D Sheet { get; private set; }
        public SheetSpecification SheetSpecification { get; private set; }

        public Dictionary<Tiles, SheetElement> SpritePositions { get; private set; }

        private readonly SpriteRenderer spriteRenderer;

        public SheetManager(SpriteRenderer spriteRenderer) {
            this.spriteRenderer = spriteRenderer;
        }

        public void LoadContent(ContentManager content) {
            string sheetSpecJson = File.ReadAllText("./Content/TileSheet.json");
            SheetSpecification = JsonConvert.DeserializeObject<SheetSpecification>(sheetSpecJson);

            Sheet = content.Load<Texture2D>("TileSheet");

            SpritePositions = new Dictionary<Tiles, SheetElement>();

            var sheetElements = SheetSpecification.Frames;
            foreach (Tiles tile in Enum.GetValues(typeof(Tiles))) {
                SpritePositions[tile] = sheetElements[tile.TileAsset() + ".png"];
            }
        }

        public void DrawTile(Tiles tile, Vector2 position, float width, float layerDepth, Action click = null, Action hover = null) {
            SheetElement spritePosition = SpritePositions[tile];

            var scale = width / spritePosition.SourceSize.Width;

            double textureLeft = spritePosition.Frame.X;
            double textureRight = textureLeft + spritePosition.Frame.Width;
            double textureTop = spritePosition.Frame.Y;
            double textureBottom = textureTop + spritePosition.Frame.Height;

            double worldLeft = spritePosition.SpriteSourceSize.X * scale + position.X - spritePosition.Pivot.X * spritePosition.SourceSize.Width * scale;
            double destinationWidth = spritePosition.SpriteSourceSize.Width * scale;
            double worldRight = worldLeft + destinationWidth;
            double worldTop = spritePosition.SpriteSourceSize.Y * scale + position.Y - spritePosition.Pivot.Y * spritePosition.SourceSize.Height * scale;
            double destinationHeight = spritePosition.SpriteSourceSize.Height * scale;
            double worldBottom = worldTop + destinationHeight;
            spriteRenderer.DrawSprite( 
                new Vector2((float)worldLeft, (float)worldTop), new Vector2((float)worldRight, (float)worldBottom), layerDepth, 
                new Vector2((float)(textureLeft / Sheet.Width), (float)(textureTop / Sheet.Height)), 
                new Vector2((float)(textureRight / Sheet.Width), (float)(textureBottom / Sheet.Height)),
                click, hover);
        }
    }
}
