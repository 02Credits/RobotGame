using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RobotGameShared {
    public class Game : Microsoft.Xna.Framework.Game {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;
        Random random;

        Color foreground = new Color(255, 176, 0);
        Color background = new Color(40, 40, 40);

        public Game() {
            graphics = new GraphicsDeviceManager(this);
#if !IOS
            graphics.PreferredBackBufferWidth = 1125 / 2;
            graphics.PreferredBackBufferHeight = 2436 / 2;
            this.IsMouseVisible = true;
#endif
            Content.RootDirectory = "Content";
        }

        protected override void Initialize() {
            random = new Random();
            base.Initialize();
        }

        protected override void LoadContent() {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("Gugi");
        }

        protected override void Update(GameTime gameTime) {
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            graphics.GraphicsDevice.Clear(background);
            spriteBatch.Begin();
            spriteBatch.DrawString(font, $"This is a random number: {random.Next() * 100000}", new Vector2(100, 100), foreground);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}

