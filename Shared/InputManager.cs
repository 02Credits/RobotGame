using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RobotGameShared {
    public class InputManager : IUpdateable {
        private readonly Game game;
        private readonly Dictionary<Color, (Action click, Action hover)> inputLookup = new Dictionary<Color, (Action click, Action hover)>();
        private Color[] inputColor = new Color[1];

        int inputId = 1;

        public Vector2? PressedLocation { get; private set; }
        public Vector2? PreviousLocation { get; private set; }
        public Vector2? PointerLocation { get; private set; }

        public bool PointerWasDown { get; private set; }
        public bool PointerDown { get; private set; }
        public bool PointerPressed => PointerDown && !PointerWasDown;
        public bool PointerReleased => !PointerDown && PointerWasDown;

        public float UpdateOrder => (float)UpdateStages.Input;

        public InputManager(Game game) {
            this.game = game;
            
        }

        public void Update(GameTime gameTime) {
            PointerWasDown = PointerDown;

#if IOS
            PointerDown = TouchPanel.GetState().Any();
#else
            PointerDown = Mouse.GetState().LeftButton == ButtonState.Pressed;
#endif
            PreviousLocation = PointerLocation;

            var possiblePointerPosition = CurrentPointerPosition();
            if (possiblePointerPosition != null) {
                var pointerPosition = possiblePointerPosition.Value;
                var pointerX = (int)(pointerPosition.X * game.PixelWidth / game.DeviceWidth);
                var pointerY = (int)(pointerPosition.Y * game.PixelHeight / game.DeviceHeight);

                if (pointerX >= 0 && pointerX < game.PixelWidth && pointerY >= 0 && pointerY < game.PixelHeight) {
                    game.InputTarget.GetData(0, new Rectangle(pointerX, pointerY, 1, 1), inputColor, 0, 1);
                    var lookupColor = inputColor[0];

                    if (inputLookup.TryGetValue(lookupColor, out var actions)) {
                        (Action click, Action hover) = actions;
                        hover?.Invoke();
                        if (PointerPressed) {
                            click?.Invoke();
                        }
                    }

                    if (PointerPressed) {
                        PressedLocation = pointerPosition;
                    }

                    PointerLocation = pointerPosition;
                } else {
                    PointerLocation = null;
                }
            } else {
                PointerLocation = null;
            }

            if (!PointerDown) {
                PressedLocation = null;
            }

            inputLookup.Clear();
            inputId = 1;
        }

        private Vector2? CurrentPointerPosition() {
#if IOS
            var touchState = TouchPanel.GetState();
            if (touchState.Any()) {
                return touchState.First().Position;
            } else {
                return null;
            }
#else
            return Mouse.GetState().Position.ToVector2();
#endif
        }

        public Color CacheInput(Action click, Action hover) {
            if (hover == null && click == null) {
                return new Color(Color.Black, 0);
            } else {
                int id = inputId++;

                int r = (id & 0x000000FF) >> 0;
                int g = (id & 0x0000FF00) >> 8;
                int b = (id & 0x00FF0000) >> 16;

                var color = new Color(r, g, b);
                inputLookup[color] = (click, hover);
                return color;
            }
        }
    }
}
