using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace RobotGameShared {
    public class ColorManager {
        public readonly IReadOnlyList<Color> Lookup = new[] {
            new Color(8, 20, 30),
            new Color(15, 42, 63),
            new Color(32, 57, 63),
            new Color(78, 73, 95),
            new Color(129, 98, 113),
            new Color(153, 117, 119),
            new Color(195, 163, 138),
            new Color(246, 214, 189)
        };
    }
}
