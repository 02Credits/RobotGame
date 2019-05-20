using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace RobotGameShared.Renderer {
    public class CameraManager : IUpdateable {
        private readonly ScreenSizeManager screenSizeManager;

        public int UpdateOrder => 0;

        public Matrix View { get; private set; }
        public Matrix Projection { get; private set; }

        public float CameraWidth { get; set; } = 10;

        public Vector3 WorldPosition { get; set; }

        public CameraManager(ScreenSizeManager screenSizeManager) {
            this.screenSizeManager = screenSizeManager;
        }

        public void Update(GameTime gameTime) {
            float heightOverWidth = (float)screenSizeManager.PixelHeight / screenSizeManager.PixelWidth;
            float cameraHeight = CameraWidth * heightOverWidth;
            Projection = Matrix.CreateOrthographicOffCenter(
                -CameraWidth / 2, CameraWidth / 2,
                -cameraHeight / 2, cameraHeight / 2,
                float.MinValue, float.MaxValue);

            View = Matrix.CreateTranslation(
                new Vector3(
                    0.5f * WorldPosition.X + 0.5f * WorldPosition.Y,
                    0.5f * WorldPosition.X - 0.5f * WorldPosition.Y + 0.25f * WorldPosition.Z,
                    0));
        }
    }
}
