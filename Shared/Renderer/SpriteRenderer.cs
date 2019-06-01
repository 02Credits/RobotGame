using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RobotGameShared.Renderer {
    public class SpriteRenderer {
        private readonly VertexManager vertexManager;
        private readonly InputManager inputManager;
        private readonly ObjectPool<SpriteRender> spritePool;
        private readonly List<SpriteRender> currentSprites = new List<SpriteRender>();

        public SpriteRenderer(VertexManager vertexManager, InputManager inputManager) {
            this.vertexManager = vertexManager;
            this.inputManager = inputManager;
            spritePool = new ObjectPool<SpriteRender>(pool => new SpriteRender(pool));
        }

        public void DrawSprite(Vector2 topLeftWorld, Vector2 bottomRightWorld, float worldZ, Vector2 topLeftTexture, Vector2 bottomRightTexture, Action click = null, Action hover = null) {
            var sprite = spritePool.Create();
            sprite.Color = inputManager.CacheInput(click, hover);
            sprite.TopLeftWorld = topLeftWorld;
            sprite.BottomRightWorld = bottomRightWorld;
            sprite.WorldZ = worldZ;
            sprite.TopLeftTexture = topLeftTexture;
            sprite.BottomRightTexture = bottomRightTexture;
            currentSprites.Add(sprite);
        }

        public void SetVertices() {
            currentSprites.Sort();
            foreach (SpriteRender sprite in currentSprites) {
                AddSprite(sprite);
                sprite.Free();
            }
            currentSprites.Clear();
        }

        private void AddSprite(SpriteRender sprite)
            => vertexManager.AddRectangle(sprite.Color,
                new Vector3(sprite.TopLeftWorld, sprite.WorldZ), new Vector3(sprite.BottomRightWorld.X, sprite.TopLeftWorld.Y, sprite.WorldZ),
                new Vector3(sprite.TopLeftWorld.X, sprite.BottomRightWorld.Y, sprite.WorldZ), new Vector3(sprite.BottomRightWorld, sprite.WorldZ),
                sprite.TopLeftTexture, new Vector2(sprite.BottomRightTexture.X, sprite.TopLeftTexture.Y),
                new Vector2(sprite.TopLeftTexture.X, sprite.BottomRightTexture.Y), sprite.BottomRightTexture);

        private class SpriteRender : Poolable<SpriteRender>, IComparable<SpriteRender> {
            public Color Color { get; set; }
            public Vector2 TopLeftWorld { get; set; }
            public Vector2 BottomRightWorld { get; set; }
            public float WorldZ { get; set; }
            public Vector2 TopLeftTexture { get; set; }
            public Vector2 BottomRightTexture { get; set; }

            public SpriteRender(ObjectPool<SpriteRender> pool) : base(pool) { }

            public override void Reset() {
                Color = Color.White;

                TopLeftWorld = Vector2.Zero;
                BottomRightWorld = Vector2.Zero;

                WorldZ = 0;

                TopLeftTexture = Vector2.Zero;
                BottomRightTexture = Vector2.Zero;
            }

            public int CompareTo(SpriteRender other) => WorldZ.CompareTo(other.WorldZ);
        }
    }
}
