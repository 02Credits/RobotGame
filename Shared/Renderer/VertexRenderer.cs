using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RobotGameShared.SpriteSheet;
using System;
using System.Collections.Generic;
using System.Text;

namespace RobotGameShared.Renderer {
    public class VertexRenderer : IDrawable {
        GraphicsDevice graphics;
        BasicEffect basicEffect;

        VertexManager vertexManager;
        private readonly CameraManager cameraManager;
        private readonly SheetManager sheetManager;

        public VertexRenderer(GraphicsDevice graphics, VertexManager vertexManager, CameraManager cameraManager, SheetManager sheetManager) {
            this.graphics = graphics;
            this.vertexManager = vertexManager;
            this.cameraManager = cameraManager;
            this.sheetManager = sheetManager;
            basicEffect = new BasicEffect(graphics);
        }

        public int DrawOrder => 2;

        public void Draw(GameTime gameTime) {
            graphics.BlendState = new BlendState {
                AlphaSourceBlend = Blend.SourceAlpha
            };
            graphics.RasterizerState = RasterizerState.CullNone;
            graphics.SamplerStates[0] = SamplerState.PointWrap;
            graphics.DepthStencilState = DepthStencilState.Default;

            basicEffect.VertexColorEnabled = true;
            basicEffect.TextureEnabled = true;

            basicEffect.World = Matrix.Identity;

            basicEffect.View = cameraManager.View;
            basicEffect.Projection = cameraManager.Projection;

            basicEffect.Texture = sheetManager.Sheet;

            var manager = vertexManager.Manager;
            if (manager.VertexCount > 0) {
                foreach (var pass in basicEffect.CurrentTechnique.Passes) {
                    pass.Apply();
                    graphics.DrawUserIndexedPrimitives(
                        PrimitiveType.TriangleList,
                        manager.Vertices,
                        0,
                        manager.VertexCount,
                        manager.Indices,
                        0,
                        manager.IndexCount / 3);
                }
            }
        }
    }
}
