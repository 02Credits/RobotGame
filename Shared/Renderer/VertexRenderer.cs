using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using RobotGameShared.SpriteSheet;
using System;
using System.Collections.Generic;
using System.Text;

namespace RobotGameShared.Renderer {
    public class VertexRenderer : IContentLoadable, IDrawable {
        private readonly Game game;
        private readonly GraphicsDevice graphics;
        private readonly SpriteBatch spriteBatch;
        private readonly SpriteRenderer spriteRenderer;
        private readonly VertexManager vertexManager;
        private readonly CameraManager cameraManager;
        private readonly SheetManager sheetManager;
        private readonly ColorManager colorManager;
        Effect effect;

        public float DrawOrder => (float)DrawStages.UI - 0.1f;

        public VertexRenderer(Game game, GraphicsDevice graphics, SpriteBatch spriteBatch, ColorManager colorManager, SpriteRenderer spriteRenderer, VertexManager vertexManager, CameraManager cameraManager, SheetManager sheetManager) {
            this.game = game;
            this.graphics = graphics;
            this.spriteBatch = spriteBatch;
            this.spriteRenderer = spriteRenderer;
            this.vertexManager = vertexManager;
            this.cameraManager = cameraManager;
            this.sheetManager = sheetManager;
            this.colorManager = colorManager;
        }

        public void LoadContent(ContentManager content) {
            effect = content.Load<Effect>("Shaders/Sprite");
        }

        public void Draw(GameTime gameTime) {
            graphics.BlendState = BlendState.AlphaBlend;
            graphics.RasterizerState = RasterizerState.CullNone;
            graphics.SamplerStates[0] = SamplerState.PointWrap;
            graphics.DepthStencilState = DepthStencilState.Default;


            effect.Parameters["View"].SetValue(cameraManager.View);
            effect.Parameters["Projection"].SetValue(cameraManager.Projection);
            effect.Parameters["SpriteSheet"].SetValue(sheetManager.Sheet);

            spriteRenderer.SetVertices();

            graphics.SetRenderTarget(game.WorldTarget);
            graphics.Clear(colorManager.Lookup[0]);
            effect.CurrentTechnique = effect.Techniques["Textured"];
            DrawVertices();
            graphics.SetRenderTarget(game.InputTarget);
            graphics.Clear(Color.Black);
            effect.CurrentTechnique = effect.Techniques["Solid"];
            DrawVertices();

            graphics.SetRenderTarget(null);
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            spriteBatch.Draw(game.WorldTarget, Vector2.Zero , Color.White);
            spriteBatch.End();
        }

        private void DrawVertices() {
            if (vertexManager.VertexCount > 0) {
                foreach (var pass in effect.CurrentTechnique.Passes) {
                    pass.Apply();
                    graphics.DrawUserIndexedPrimitives(
                        PrimitiveType.TriangleList,
                        vertexManager.Vertices,
                        0,
                        vertexManager.VertexCount,
                        vertexManager.Indices,
                        0,
                        vertexManager.IndexCount / 3);
                }
            }
        }
    }
}
