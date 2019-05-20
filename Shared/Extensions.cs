using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace RobotGameShared {
    public static class Extensions {
        public static T? ElementAt<T>(this T?[][] grid, Vector2 position) where T : struct {
            int xIndex = (int)position.X;
            if (xIndex >= 0 && xIndex < grid.Length) {
                T?[] column = grid[xIndex];
                int yIndex = (int)position.Y;
                if (yIndex >= 0 && yIndex < column.Length) {
                    return column[yIndex];
                }
            }

            return null;
        }

        public static T? ElementAt<T>(this T[][] grid, Vector2 position) where T : struct {
            int xIndex = (int)position.X;
            if (xIndex >= 0 && xIndex < grid.Length) {
                T[] column = grid[xIndex];
                int yIndex = (int)position.Y;
                if (yIndex >= 0 && yIndex < column.Length) {
                    return column[yIndex];
                }
            }

            return null;
        }

        public static void SetElementAt<T>(this T[][] grid, Vector2 position, T value) {
            int xIndex = (int)position.X;
            int yIndex = (int)position.Y;
            grid[xIndex][yIndex] = value;
        }
    }
}
