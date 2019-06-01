using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace RobotGameShared.Renderer {
    public class VertexManager : IUpdateable {
        public static VertexPositionColorTexture vertexToAdd;
        public VertexPositionColorTexture[] Vertices = new VertexPositionColorTexture[4000];
        public short[] Indices = new short[4000];

        public int VertexCount { get; private set; }

        public int IndexCount { get; private set; }

        private short CurrentIndex { get; set; }

        private int CurrentMaxVertexCount { get; set; }

        private int CurrentMaxIndexCount { get; set; }

        public float UpdateOrder => (float)UpdateStages.Setup;

        public void AddRectangle(Color color,
                Vector3 topLeftWorld, Vector3 topRightWorld, Vector3 bottomLeftWorld, Vector3 bottomRightWorld,
                Vector2 topLeftTexture, Vector2 topRightTexture, Vector2 bottomLeftTexture, Vector2 bottomRightTexture) {
            AddVertex(topLeftWorld, topLeftTexture, color);
            AddVertex(topRightWorld, topRightTexture, color);
            AddVertex(bottomRightWorld, bottomRightTexture, color);
            AddVertex(bottomLeftWorld, bottomLeftTexture, color);
            AddIndex(CurrentIndex);
            AddIndex((short)(CurrentIndex + 1));
            AddIndex((short)(CurrentIndex + 2));
            AddIndex((short)(CurrentIndex + 2));
            AddIndex((short)(CurrentIndex + 3));
            AddIndex(CurrentIndex);

            CurrentIndex += 4;
        }

        public void AddVertex(Vector3 position, Vector2 texturePosition, Color color) {
            VertexCount += 1;

            ExtendVertexArrayIfNeeded();

            vertexToAdd.Position = position;
            vertexToAdd.TextureCoordinate = texturePosition;
            vertexToAdd.Color = color;
            Vertices[VertexCount - 1] = vertexToAdd;
        }

        public void AddIndex(short index) {
            IndexCount += 1;

            ExtendIndexArrayIfNeeded();

            Indices[IndexCount - 1] = index;
        }

        private void ExtendVertexArrayIfNeeded() {
            if (VertexCount > CurrentMaxVertexCount) {
                CurrentMaxVertexCount = VertexCount;
                if (CurrentMaxVertexCount > Vertices.Length) {
                    var newArray = new VertexPositionColorTexture[CurrentMaxVertexCount + 500];
                    Vertices.CopyTo(newArray, 0);
                    Vertices = newArray;
                }
            }
        }

        private void ExtendIndexArrayIfNeeded() {
            if (IndexCount > CurrentMaxIndexCount) {
                CurrentMaxIndexCount = IndexCount;
                if (CurrentMaxIndexCount > Indices.Length) {
                    var newArray = new Int16[CurrentMaxIndexCount + 500];
                    Indices.CopyTo(newArray, 0);
                    Indices = newArray;
                }
            }
        }

        public void Update(GameTime gameTime) {
            VertexCount = 0;
            IndexCount = 0;
            CurrentIndex = 0;
        }
    }
}
