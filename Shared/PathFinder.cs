using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using RobotGameShared.Map;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RobotGameShared {
    public class PathFinder {
        private readonly MapManager mapManager;
        private readonly WorldRenderer worldRenderer;

        public Vector2 Target { get; set; } = Vector2.Zero;

        public PathFinder(MapManager mapManager, WorldRenderer worldRenderer) {
            this.mapManager = mapManager;
            this.worldRenderer = worldRenderer;
        }

        public IEnumerable<Vector2> ReachableAdjacentLocations(Vector2 position) {
            return mapManager
                .TileAt(position).Value
                .WalkableDirections()
                .Select(direction => position + direction.UnitVector())
                .Where(adjacentPosition => {
                    Tiles? tile = mapManager.TileAt(adjacentPosition);
                    return tile.HasValue && tile.Value.Walkable() && mapManager.EntityAt(adjacentPosition) == null;
                });
        }

        public IReadOnlyList<Vector2> FindShortestPath(Vector2 from, Vector2 to) {
            int?[][] distanceLookup = BuildDistanceMap(from, to);

            Vector2 currentLocation = from;
            HashSet<Vector2> previousLocations = new HashSet<Vector2>();
            
            IEnumerable<Vector2> FollowGradient() {
                while (currentLocation != to && !previousLocations.Contains(currentLocation)) {
                    yield return currentLocation;
                    previousLocations.Add(currentLocation);

                    List<Vector2> reachableAdjacentLocations = ReachableAdjacentLocations(currentLocation).ToList();
                    if (!reachableAdjacentLocations.Any()) yield break;
                    currentLocation = reachableAdjacentLocations
                        .OrderBy(adjacentLocation => distanceLookup.ElementAt(adjacentLocation))
                        .First();
                }

                if (currentLocation == to) {
                    yield return to;
                }
            }

            List<Vector2> tracedPath = FollowGradient().ToList();
            return tracedPath.Last() == to ? tracedPath : null;
        }

        private int?[][] BuildDistanceMap(Vector2 from, Vector2 to) {
            var nextLocations = new Queue<Vector2>();

            var distanceLookup = new int?[mapManager.Radius][];
            for (int x = 0; x < mapManager.Radius; x++) {
                distanceLookup[x] = new int?[mapManager.Radius];
            }

            int currentDistance = 0;
            nextLocations.Enqueue(to);
            distanceLookup.SetElementAt(to, currentDistance);

            while (nextLocations.Any()) {
                var locationsToConcider = nextLocations;
                nextLocations = new Queue<Vector2>();
                currentDistance++;

                while (locationsToConcider.Any()) {
                    Vector2 nextLocation = locationsToConcider.Dequeue();

                    foreach (Vector2 reachableAdjacentLocation in ReachableAdjacentLocations(nextLocation)) {
                        if (reachableAdjacentLocation == from) return distanceLookup;
                        if (distanceLookup.ElementAt(reachableAdjacentLocation) == null) {
                            distanceLookup.SetElementAt(reachableAdjacentLocation, currentDistance);
                            nextLocations.Enqueue(reachableAdjacentLocation);
                        }
                    }
                }
            }

            return distanceLookup;
        }

        public void DrawPath(IReadOnlyList<Vector2> path) {
            foreach ((Vector2 previous, Vector2 current) in path.Skip(1).Zip(path.Skip(2), (a, b) => (a, b))) {
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
