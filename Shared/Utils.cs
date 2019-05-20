using System;
using System.Collections.Generic;
using System.Text;

namespace RobotGameShared {
    public static class Utils {
        public static T[][] Transpose<T>(this T[][] grid) {
            int resultingWidth = grid[0].Length;
            int resultingHeight = grid.Length;

            T[][] result = new T[resultingWidth][];

            for (var x = 0; x < resultingWidth; x++) {
                result[x] = new T[resultingHeight];
                for (var y = 0; y < resultingHeight; y++) {
                    result[x][y] = grid[y][x];
                }
            }

            return result;
        }

        public static T[][] FlipHorizontally<T>(this T[][] grid) {
            int width = grid.Length;
            int height = grid[0].Length;
            T[][] result = new T[width][];

            for (var x = 0; x < width; x++) {
                result[x] = new T[height];
                for (var y = 0; y < height; y++) {
                    result[x][y] = grid[width - x - 1][y];
                }
            }

            return result;
        }
    }
}
