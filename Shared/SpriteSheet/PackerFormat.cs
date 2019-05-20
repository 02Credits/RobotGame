using System;
using System.Collections.Generic;
using System.Text;

namespace RobotGameShared.SpriteSheet {
    public class Frame {
        public int X { get; }
        public int Y { get; }
        public int Width { get; }
        public int Height { get; }

        public Frame(int x, int y, int w, int h) {
            X = x;
            Y = y;
            Width = w;
            Height = h;
        }
    }

    public class Position {
        public float X { get; }
        public float Y { get; }

        public Position(float x, float y) {
            X = x;
            Y = y;
        }
    }

    public class Dimensions {
        public int Width { get; }
        public int Height { get; }

        public Dimensions(int w, int h) {
            Width = w;
            Height = h;
        }
    }

    public class SheetElement {
        public Frame Frame { get; }
        public bool Rotated { get; }
        public Frame SpriteSourceSize { get; }
        public Dimensions SourceSize { get; }
        public Position Pivot { get; }

        public SheetElement(Frame frame, bool rotated, Frame spriteSourceSize, Dimensions sourceSize, Position pivot) {
            Frame = frame;
            Rotated = rotated;
            SpriteSourceSize = spriteSourceSize;
            SourceSize = sourceSize;
            Pivot = pivot;
        }
    }

    public class SheetSpecification {
        public IReadOnlyDictionary<string, SheetElement> Frames { get; }

        public SheetSpecification(Dictionary<string, SheetElement> frames) {
            Frames = frames;
        }
    }
}
