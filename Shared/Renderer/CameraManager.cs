using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace RobotGameShared.Renderer {
    public class CameraManager : IUpdateable {
        private readonly Game game;

        public float UpdateOrder => (float)UpdateStages.Setup;

        public Matrix View { get; private set; }
        public Matrix Projection { get; private set; }

        public float MinTileSize { get; set; } = 6;

        public Vector3 WorldPosition { get; set; }

        public CameraManager(Game game) {
            this.game = game;
        }

        public void Update(GameTime gameTime) {
            float cameraWidth;
            float cameraHeight;

            if (game.PixelWidth < game.PixelHeight * 2) {
                float heightOverWidth = (float)game.PixelHeight / game.PixelWidth;
                cameraWidth = MinTileSize;
                cameraHeight = cameraWidth * heightOverWidth;
            } else {
                float widthOverHeight = (float)game.PixelWidth / game.PixelHeight;
                cameraHeight = MinTileSize / 2;
                cameraWidth = cameraHeight * widthOverHeight;
            }


            Projection = Matrix.CreateOrthographicOffCenter(
                -cameraWidth / 2, cameraWidth / 2,
                cameraHeight / 2, -cameraHeight / 2,
                float.MinValue, float.MaxValue);

            View = Matrix.CreateTranslation(
                new Vector3(
                    -WorldPosition.X - WorldPosition.Y,
                    WorldPosition.X / 2.0f - WorldPosition.Y / 2.0f + WorldPosition.Z / 2.0f,
                    0) / 2.0f);
        }
    }
}
