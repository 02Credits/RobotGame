using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace RobotGameShared {
    public class ScreenSizeManager {
        private Game game;

        public int PixelWidth => game.RenderTarget.Width;
        public int PixelHeight => game.RenderTarget.Height;

        public int DeviceWidth => game.GraphicsDevice.PresentationParameters.BackBufferWidth;
        public int DeviceHeight => game.GraphicsDevice.PresentationParameters.BackBufferWidth;

        public ScreenSizeManager(Game game) {
            this.game = game;
        }
    }
}
