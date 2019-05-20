using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace RobotGameShared.Map {
    public enum Directions {
        NegativeX,
        NegativeY,
        PositiveX,
        PositiveY,
    };

    public enum Axes {
        X, Y
    }

    public static class GeometryExtensions {
        public static readonly IReadOnlyCollection<Directions> AllDirections = new[] {
            Directions.PositiveX, Directions.PositiveY, Directions.NegativeX, Directions.NegativeY
        };

        public static readonly IReadOnlyCollection<Axes> AllAxes = new[] {
            Axes.X, Axes.Y
        };

        public static readonly IReadOnlyDictionary<Axes, IReadOnlyCollection<Directions>> AlignedDirectionsLookup = new Dictionary<Axes, IReadOnlyCollection<Directions>> {
            [Axes.X] = new HashSet<Directions> { Directions.PositiveX, Directions.NegativeX },
            [Axes.Y] = new HashSet<Directions> { Directions.PositiveY, Directions.NegativeY }
        };

        public static IReadOnlyCollection<Directions> AlignedDirections(this Axes axis) => AlignedDirectionsLookup[axis];

        public static readonly IReadOnlyDictionary<Directions, Vector2> DirectionUnitVectorsLookup = new Dictionary<Directions, Vector2> {
            [Directions.NegativeX] = new Vector2(-1, 0),
            [Directions.NegativeY] = new Vector2(0, -1),
            [Directions.PositiveX] = new Vector2(1, 0),
            [Directions.PositiveY] = new Vector2(0, 1)
        };

        public static Vector2 UnitVector(this Directions direction) => DirectionUnitVectorsLookup[direction];

        public static readonly IReadOnlyDictionary<Vector2, Directions> UnitVectorDirectionLookup = new Dictionary<Vector2, Directions> {
            [DirectionUnitVectorsLookup[Directions.NegativeX]] = Directions.NegativeX,
            [DirectionUnitVectorsLookup[Directions.NegativeY]] = Directions.NegativeY,
            [DirectionUnitVectorsLookup[Directions.PositiveX]] = Directions.PositiveX,
            [DirectionUnitVectorsLookup[Directions.PositiveY]] = Directions.PositiveY
        };

        public static Directions Direction(this Vector2 vector) {
            vector.Normalize();
            return UnitVectorDirectionLookup[vector];
        }
    }
}
