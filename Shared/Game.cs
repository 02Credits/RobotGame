using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace RobotGameShared {

    interface IContentLoadable {
        void LoadContent(ContentManager content);
    }

    interface IUpdateable {
        void Update(GameTime gameTime);
    }

    interface IDrawable {
        void Draw(GameTime gameTime);
    }

    public class Game : Microsoft.Xna.Framework.Game {
        IList<IContentLoadable> contentLoadables;
        IList<IUpdateable> updateables;
        IList<IDrawable> drawables;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Colors colorManager;

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

            base.Initialize();
        }

        protected override void LoadContent() {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Factory.BeginRegistration();
            Factory.RegisterSingleton(GraphicsDevice);
            Factory.RegisterSingleton(spriteBatch);
            Factory.EndRegistration();

            contentLoadables = Factory.Resolve<IList<IContentLoadable>>();
            updateables = Factory.Resolve<IList<IUpdateable>>();
            drawables = Factory.Resolve<IList<IDrawable>>();
            colorManager = Factory.Resolve<Colors>();

            foreach (IContentLoadable contentLoadable in contentLoadables) {
                contentLoadable.LoadContent(Content);
            }
        }

        protected override void Update(GameTime gameTime) {
            base.Update(gameTime);

            foreach (IUpdateable updateable in updateables) {
                updateable.Update(gameTime);
            }
        }

        protected override void Draw(GameTime gameTime) {
            graphics.GraphicsDevice.Clear(colorManager.Background);

            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            foreach (IDrawable drawable in drawables) {
                drawable.Draw(gameTime);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

