using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace RobotGameShared {
    interface IContentLoadable {
        void LoadContent(ContentManager content);
    }

    interface IUpdateable {
        int UpdateOrder { get; }
        void Update(GameTime gameTime);
    }

    interface IDrawable {
        int DrawOrder { get; }
        void Draw(GameTime gameTime);
    }

    public class Game : Microsoft.Xna.Framework.Game {
        IList<IContentLoadable> contentLoadables;
        IList<IUpdateable> updateables;
        IList<IDrawable> drawables;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Colors colorManager;

        public RenderTarget2D RenderTarget { get; private set; }

        public Game() {
            graphics = new GraphicsDeviceManager(this);
#if !IOS
            graphics.PreferredBackBufferWidth = 1125 / 2;
            graphics.PreferredBackBufferHeight = 2436 / 2;
            this.IsMouseVisible = true;
            Window.AllowUserResizing = true;
#endif

            Content.RootDirectory = "Content";
        }

        protected override void Initialize() {
            base.Initialize();
        }

        protected override void LoadContent() {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            ResizeRenderTarget();

            Factory.BeginRegistration();
            Factory.RegisterSingleton(new Random());
            Factory.RegisterSingleton(graphics);
            Factory.RegisterSingleton(GraphicsDevice);
            Factory.RegisterSingleton(spriteBatch);
            Factory.RegisterSingleton(this);
            Factory.EndRegistration();

            contentLoadables = Factory.Resolve<IList<IContentLoadable>>();
            updateables = Factory.Resolve<IEnumerable<IUpdateable>>().OrderBy(updateable => updateable.UpdateOrder).ToList();
            drawables = Factory.Resolve<IEnumerable<IDrawable>>().OrderBy(drawable => drawable.DrawOrder).ToList();

            colorManager = Factory.Resolve<Colors>();

            foreach (IContentLoadable contentLoadable in contentLoadables) {
                contentLoadable.LoadContent(Content);
            }
        }

        protected override void Update(GameTime gameTime) {
            base.Update(gameTime);

#if !IOS
            if (graphics.PreferredBackBufferWidth != Window.ClientBounds.Width ||
                graphics.PreferredBackBufferHeight != Window.ClientBounds.Height) {
                graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
                graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
                graphics.ApplyChanges();
                ResizeRenderTarget();
            }
#endif

            foreach (IUpdateable updateable in updateables) {
                updateable.Update(gameTime);
            }
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.SetRenderTarget(RenderTarget);
            graphics.GraphicsDevice.Clear(colorManager.Lookup[0]);

            foreach (IGrouping<int, IDrawable> drawablesLayer in drawables.GroupBy(drawable => drawable.DrawOrder)) {
                spriteBatch.Begin(samplerState: SamplerState.PointClamp, sortMode: SpriteSortMode.FrontToBack);
                foreach (IDrawable drawable in drawablesLayer) {
                    drawable.Draw(gameTime);
                }
                spriteBatch.End();
            }

            GraphicsDevice.SetRenderTarget(null);
            spriteBatch.Begin(samplerState: SamplerState.PointClamp, sortMode: SpriteSortMode.FrontToBack);
            spriteBatch.Draw(RenderTarget, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void ResizeRenderTarget() {
            RenderTarget = new RenderTarget2D(
                GraphicsDevice,
                352, 352 * GraphicsDevice.PresentationParameters.BackBufferHeight / GraphicsDevice.PresentationParameters.BackBufferWidth,
                false, GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);
        }
    }
}

