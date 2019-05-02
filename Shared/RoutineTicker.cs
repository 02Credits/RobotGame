using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace RobotGameShared {
    public class RoutineTicker : IContentLoadable, IDrawable {
        AnimationManager animationManager;
        Colors colors;
        SpriteBatch spriteBatch;

        SpriteFont font;
        List<Texture2D> actions = new List<Texture2D>();

        public RoutineTicker(AnimationManager animationManager, Colors colors, SpriteBatch spriteBatch) {
            this.animationManager = animationManager;
            this.colors = colors;
            this.spriteBatch = spriteBatch;
        }

        public void LoadContent(ContentManager content) {
            font = content.Load<SpriteFont>("Gugi");

            actions.Add(content.Load<Texture2D>("Actions/Cloud"));
            actions.Add(content.Load<Texture2D>("Actions/Leaf"));
            actions.Add(content.Load<Texture2D>("Actions/Moon"));
            actions.Add(content.Load<Texture2D>("Actions/Rain"));
            actions.Add(content.Load<Texture2D>("Actions/Stick"));
            actions.Add(content.Load<Texture2D>("Actions/Sun"));
        }

        public void Draw(GameTime gameTime) {
            spriteBatch.DrawString(font, "Hello World!", new Vector2(100, 100), colors.Foreground);

            for (var i = 0; i < 5; i++) {
                int yPosition = animationManager.Interpolate("Test", gameTime.TotalGameTime.TotalSeconds + 3.1415 * i, 600, 300);
                spriteBatch.Draw(actions[i], new Rectangle(i * 100 + 40, yPosition, 80, 80), Color.White);
            }
        }
    }
}
