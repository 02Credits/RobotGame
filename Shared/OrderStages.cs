using System;
using System.Collections.Generic;
using System.Text;

namespace RobotGameShared {
    public enum UpdateStages {
        Setup = 0,
        Input  = 1,
        Cleanup = int.MaxValue
    }

    public enum DrawStages {
        Background = 0,
        World = 50,
        UI = 100,
        Cleanup = int.MaxValue
    }
}
