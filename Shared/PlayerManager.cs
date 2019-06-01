using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using RobotGameShared.Map;
using RobotGameShared.Renderer;

namespace RobotGameShared {
    public enum PlayerState {
        Standing,
        Walking
    }

    public class PlayerManager : IUpdateable, IDrawable {
        private readonly MapManager mapManager;
        private readonly WorldRenderer worldRenderer;
        private readonly PathFinder pathFinder;
        private readonly CameraManager cameraManager;
        private readonly AnimationManager animationManager;
        private readonly InputManager inputManager;

        public PlayerState CurrentState { get; private set; }
        public Vector2 PlayerPosition { get; private set; }

        private Vector2 _nextPosition;
        private Vector2 _destination;


        public float UpdateOrder => (float)UpdateStages.Input + 0.1f;
        public float DrawOrder => (float)DrawStages.World;

        public PlayerManager(MapManager mapManager, WorldRenderer worldRenderer, PathFinder pathFinder, CameraManager cameraManager, AnimationManager animationManager, InputManager inputManager, Random random) {
            this.mapManager = mapManager;
            this.worldRenderer = worldRenderer;
            this.pathFinder = pathFinder;
            this.cameraManager = cameraManager;
            this.animationManager = animationManager;
            this.inputManager = inputManager;
            SetPlayerPosition(random);
        }

        public void Update(GameTime gameTime) {
            cameraManager.WorldPosition = mapManager.GroundLevel(PlayerPosition);

            if (inputManager.PointerPressed) {
                _destination = worldRenderer.InputWorldPosition ?? _destination;
            }
        }

        public void Draw(GameTime gameTime) {
            var shortestPath = pathFinder.FindShortestPath(PlayerPosition, _destination);
            if (shortestPath != null) {
                pathFinder.DrawPath(shortestPath);
            }
        }

        public void SetPlayerPosition(Random random) {
            while (true) {
                int x = random.Next(mapManager.Width - 1);
                int y = random.Next(mapManager.Height - 1);
                if (mapManager.TileMap[x][y].tile == Tiles.Flat && mapManager.EntityMap[x][y] == null) {
                    mapManager.EntityMap[x][y] = Tiles.Ball;
                    PlayerPosition = new Vector2(x, y);
                    break;
                }
            }
            _destination = PlayerPosition;
        }
    }
}
