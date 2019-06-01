using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace RobotGameShared.UI {
    public class UIManager : IContentLoadable, IDrawable {
        private Texture2D buttonPressed;
        private Texture2D buttonUnpressed;
        private readonly Game game;
        private readonly GraphicsDevice graphicsDevice;
        private readonly SpriteBatch spriteBatch;
        private readonly InputManager inputManager;

        private Vector2 inputOffset = Vector2.Zero;

        public float DrawOrder => (float)DrawStages.UI;

        public UIManager(Game game, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, InputManager inputManager) {
            this.game = game;
            this.graphicsDevice = graphicsDevice;
            this.spriteBatch = spriteBatch;
            this.inputManager = inputManager;
        }

        public void LoadContent(ContentManager content) {
            buttonPressed = content.Load<Texture2D>("UI/ButtonPressed");
            buttonUnpressed = content.Load<Texture2D>("UI/ButtonUnpressed");
        }

        (float scrollTop, float scrollVelocity) scrollState = (0, 0);
        public void Draw(GameTime gameTime) {
            var scrollArea = new Rectangle((int)(game.PixelWidth * 0.25), (int)(game.PixelWidth * 0.25), (int)(game.PixelWidth * 0.5), (int)(game.PixelHeight - game.PixelWidth * 0.5));

            int buttonCount = 20;
            using (VerticalScrollView(scrollArea, 100 * buttonCount - 10, ref scrollState)) {
                for (int i = 0; i < buttonCount; i++) {
                    if (Button(new Rectangle(0, i * 100, 90, 90), buttonUnpressed, buttonPressed)) {
                        Console.WriteLine($"Button {i + 1} pressed.");
                    }
                }
            }
        }

        public bool Button(Rectangle destination, Texture2D unpressed, Texture2D pressed) {
            spriteBatch.Begin(samplerState: SamplerState.PointClamp, rasterizerState: new RasterizerState() { ScissorTestEnable = true });
            Texture2D buttonTexture = unpressed;
            bool result = false;

            Vector2? possiblePointerPosition = inputManager.PointerLocation;
            if (possiblePointerPosition.HasValue && graphicsDevice.ScissorRectangle.Contains(possiblePointerPosition.Value) && destination.Contains(possiblePointerPosition.Value - inputOffset))  {
                if (inputManager.PointerDown) {
                    buttonTexture = pressed;
                } else if (inputManager.PointerReleased) {
                    result = true;
                }
            }

            spriteBatch.Draw(buttonTexture, destination, Color.White);
            spriteBatch.End();
            return result;
        }

        public IDisposable VerticalScrollView(Rectangle scrollArea, float scrollHeight, ref (float scrollTop, float scrollVelocity) state) {
            float velocity = state.scrollVelocity;
            if (inputManager.PreviousLocation.HasValue && inputManager.PressedLocation.HasValue && scrollArea.Contains(inputManager.PressedLocation.Value)) {
                velocity = inputManager.PointerLocation.Value.Y - inputManager.PreviousLocation.Value.Y;
            } else {
                velocity = velocity * 0.9f;
            }

            float scrollTop = state.scrollTop - velocity;
            if (scrollTop < 0) scrollTop = 0;
            if (scrollTop >= scrollHeight - scrollArea.Height) scrollTop = scrollHeight - scrollArea.Height;
            state = (scrollTop, velocity);

            Viewport oldViewport = graphicsDevice.Viewport;
            graphicsDevice.Viewport = new Viewport(new Rectangle(scrollArea.X, (int)(scrollArea.Y - state.scrollTop), scrollArea.Width, (int)scrollHeight));
            Rectangle oldScissor = graphicsDevice.ScissorRectangle;
            graphicsDevice.ScissorRectangle = scrollArea;


            Vector2 offset = new Vector2(graphicsDevice.Viewport.X, graphicsDevice.Viewport.Y);
            inputOffset += offset;

            return new AnonymousDisposable(() => {
                graphicsDevice.Viewport = oldViewport;
                graphicsDevice.ScissorRectangle = oldScissor;
                inputOffset -= offset;
            });
        }
    }
}
