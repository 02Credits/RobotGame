using Microsoft.Xna.Framework;
using RobotGameShared.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RobotGameShared.Pathing {
    public class PathRenderer {
        private readonly MapManager mapManager;
        private readonly WorldRenderer worldRenderer;

        public PathRenderer(MapManager mapManager, WorldRenderer worldRenderer) {
            this.mapManager = mapManager;
            this.worldRenderer = worldRenderer;
        }

        public void DrawPath(IReadOnlyList<Vector2> path) {
            foreach ((Vector2 previous, Vector2 current) in path.Zip(path.Skip(1), (a, b) => (a, b))) {
                Directions direction = (current - previous).Direction();
                Tiles previousTile = mapManager.TileAt(previous).Value;
                Tiles arrow;
                if (previousTile == Tiles.SlopeLeft || previousTile == Tiles.SlopeRight) {
                    arrow = direction.SlopeArrow();
                } else {
                    arrow = direction.FlatArrow();
                }

                worldRenderer.DrawEntity(arrow, previous);
            }
        }
    }
}
