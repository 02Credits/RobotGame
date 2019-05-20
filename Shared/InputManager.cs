using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace RobotGameShared {
    public class InputManager {
        private readonly ScreenSizeManager screenSizeManager;

        public InputManager(ScreenSizeManager screenSizeManager) {
            this.screenSizeManager = screenSizeManager;
        }

        public Vector2 GetInputPosition() {
            var mouseState = Mouse.GetState();

            return new Vector2(
                mouseState.Position.X * screenSizeManager.PixelWidth / screenSizeManager.DeviceWidth,
                mouseState.Position.Y * screenSizeManager.PixelHeight / screenSizeManager.DeviceHeight);
        }
    }
}
