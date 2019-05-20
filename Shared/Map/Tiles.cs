using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RobotGameShared.Map {
    public enum Tiles {
        Flat,
        Raised,

        SlopeRight,
        SlopeLeft,
        SlopeUp,
        SlopeDown,

        Wall,
        Tree,
        Ball,

        ArrowNegativeX,
        ArrowNegativeY,
        ArrowPositiveX,
        ArrowPositiveY,

        ArrowSlopeNegativeX,
        ArrowSlopeNegativeY,
        ArrowSlopePositiveX,
        ArrowSlopePositiveY,
    }

    public static class TilesExtensions {
        private static readonly IReadOnlyCollection<Tiles> walkableTerrain = new HashSet<Tiles> {
            Tiles.Flat, Tiles.SlopeRight, Tiles.SlopeLeft
        };

        public static bool Walkable(this Tiles tile) => walkableTerrain.Contains(tile);

        private static readonly IReadOnlyDictionary<Tiles, IReadOnlyCollection<Directions>> walkableDirections = new Dictionary<Tiles, IReadOnlyCollection<Directions>> {
            [Tiles.Flat] = GeometryExtensions.AllDirections,
            [Tiles.SlopeRight] = Axes.X.AlignedDirections(),
            [Tiles.SlopeLeft] = Axes.Y.AlignedDirections()
        };

        public static IReadOnlyCollection<Directions> WalkableDirections(this Tiles tile) => walkableDirections.ContainsKey(tile) ? walkableDirections[tile] : new Directions[0];

        public static bool CanWalkToward(this Tiles tile, Directions direction) => tile.WalkableDirections().Contains(direction);

        private static readonly IReadOnlyDictionary<Directions, Tiles> FlatArrowLookup = new Dictionary<Directions, Tiles> {
            [Directions.NegativeX] = Tiles.ArrowNegativeX,
            [Directions.NegativeY] = Tiles.ArrowNegativeY,
            [Directions.PositiveX] = Tiles.ArrowPositiveX,
            [Directions.PositiveY] = Tiles.ArrowPositiveY,
        };

        public static Tiles FlatArrow(this Directions direction) => FlatArrowLookup[direction];

        private static readonly IReadOnlyDictionary<Directions, Tiles> SlopeArrowLookup = new Dictionary<Directions, Tiles> {
            [Directions.NegativeX] = Tiles.ArrowSlopeNegativeX,
            [Directions.NegativeY] = Tiles.ArrowSlopeNegativeY,
            [Directions.PositiveX] = Tiles.ArrowSlopePositiveX,
            [Directions.PositiveY] = Tiles.ArrowSlopePositiveY,
        };

        public static Tiles SlopeArrow(this Directions direction) => SlopeArrowLookup[direction];
    }
}
