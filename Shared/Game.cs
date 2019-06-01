using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RobotGameShared {
    interface IContentLoadable {
        void LoadContent(ContentManager content);
    }

    interface IUpdateable {
        float UpdateOrder { get; }
        void Update(GameTime gameTime);
    }

    interface IDrawable {
        float DrawOrder { get; }
        void Draw(GameTime gameTime);
    }

    public class Game : Microsoft.Xna.Framework.Game {
        IList<IContentLoadable> contentLoadables;
        IList<IUpdateable> updateables;
        IList<IDrawable> drawables;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public RenderTarget2D WorldTarget { get; private set; }
        public RenderTarget2D InputTarget { get; private set; }

        public int PixelWidth => WorldTarget.Width;
        public int PixelHeight => WorldTarget.Height;

#if IOS
        public int DeviceWidth => Window.ClientBounds.Width;
        public int DeviceHeight => Window.ClientBounds.Height;
#else
        public int DeviceWidth => GraphicsDevice.Viewport.Width;
        public int DeviceHeight => GraphicsDevice.Viewport.Height;
#endif

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

            DepthStencilState depthBufferState = new DepthStencilState(); 
            depthBufferState.DepthBufferEnable = true;
            GraphicsDevice.DepthStencilState = depthBufferState;
        }

        protected override void LoadContent() {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            WorldTarget = ResizeRenderTarget();
            InputTarget = ResizeRenderTarget();

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

            foreach (IContentLoadable contentLoadable in contentLoadables) {
                contentLoadable.LoadContent(Content);
            }
        }

        protected override void Update(GameTime gameTime) {
            base.Update(gameTime);

            if (graphics.PreferredBackBufferWidth != DeviceWidth ||
                graphics.PreferredBackBufferHeight != DeviceHeight) {
                graphics.PreferredBackBufferWidth = DeviceWidth;
                graphics.PreferredBackBufferHeight = DeviceHeight;
                graphics.ApplyChanges();
                WorldTarget = ResizeRenderTarget();
                InputTarget = ResizeRenderTarget();
            }

            foreach (IUpdateable updateable in updateables) {
                updateable.Update(gameTime);
            }
        }

        protected override void Draw(GameTime gameTime) {
            foreach (IDrawable drawable in drawables) {
                drawable.Draw(gameTime);
            }

            base.Draw(gameTime);
        }

        private RenderTarget2D ResizeRenderTarget() {
            return new RenderTarget2D(
                GraphicsDevice,
                DeviceWidth, DeviceHeight,
                false, GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);
        }
    }
}

