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

        public readonly IReadOnlyDictionary<Tiles, string> TileAssets = new Dictionary<Tiles, string> {
            [Tiles.Flat] = "U0R0D0L0",
            [Tiles.Raised] = "U1R1D1L1",

            [Tiles.SlopeRight] = "U1R1D0L0",
            [Tiles.SlopeLeft] = "U1R0D0L1",
            [Tiles.SlopeUp] = "U1R0D0L0",
            [Tiles.SlopeDown] = "U1R1D0L1",

            [Tiles.Wall] = "Wall",
            [Tiles.Tree] = "Tree",
            [Tiles.Ball] = "Ball",

            [Tiles.ArrowNegativeX] = "FlatNegativeX",
            [Tiles.ArrowNegativeY] = "FlatNegativeY",
            [Tiles.ArrowPositiveX] = "FlatPositiveX",
            [Tiles.ArrowPositiveY] = "FlatPositiveY",

            [Tiles.ArrowSlopeNegativeX] = "SlopeNegativeX",
            [Tiles.ArrowSlopeNegativeY] = "SlopeNegativeY",
            [Tiles.ArrowSlopePositiveX] = "SlopePositiveX",
            [Tiles.ArrowSlopePositiveY] = "SlopePositiveY",
        };

        private readonly SpriteBatch spriteBatch;
        private readonly VertexManager vertexManager;

        public SheetManager(SpriteBatch spriteBatch, VertexManager vertexManager) {
            this.spriteBatch = spriteBatch;
            this.vertexManager = vertexManager;
        }

        public void LoadContent(ContentManager content) {
            string sheetSpecJson = File.ReadAllText("./Content/TileSheet.json");
            SheetSpecification = JsonConvert.DeserializeObject<SheetSpecification>(sheetSpecJson);

            Sheet = content.Load<Texture2D>("TileSheet");

            SpritePositions = new Dictionary<Tiles, SheetElement>();

            var sheetElements = SheetSpecification.Frames;
            foreach (Tiles tile in Enum.GetValues(typeof(Tiles))) {
                SpritePositions[tile] = sheetElements[TileAssets[tile] + ".png"];
            }
        }

        public void DrawTile(Tiles tile, Vector2 position, float width, Color color, float layerDepth) {
            SheetElement spritePosition = SpritePositions[tile];

            var scale = width / spritePosition.SourceSize.Width;

            int textureLeft = spritePosition.Frame.X;
            int textureRight = textureLeft + spritePosition.Frame.Width;
            int textureTop = spritePosition.Frame.Y;
            int textureBottom = textureTop + spritePosition.Frame.Height;

            float worldLeft = spritePosition.SpriteSourceSize.X * scale + position.X - spritePosition.Pivot.X * spritePosition.SourceSize.Width * scale;
            float destinationWidth = spritePosition.SpriteSourceSize.Width * scale;
            float worldRight = worldLeft + destinationWidth;
            float worldTop = spritePosition.SpriteSourceSize.Y * scale + position.Y - spritePosition.Pivot.Y * spritePosition.SourceSize.Height * scale;
            float destinationHeight = spritePosition.SpriteSourceSize.Height * scale;
            float worldBottom = worldTop + destinationHeight;
            vertexManager.AddRectangle(color, new Vector2(worldLeft, worldTop), new Vector2(worldRight, worldBottom), layerDepth, new Vector2(textureLeft, textureTop), new Vector2(textureRight, textureBottom));
            /*
            var destinationRectangle = new Rectangle(
                ,
                (int)(),
                (int)destinationWidth,
                (int)destinationHeight);

            spriteBatch.Draw(Sheet, destinationRectangle, sourceRectangle, color, 0, Vector2.Zero, SpriteEffects.None, layerDepth);*/


        }
    }
}
