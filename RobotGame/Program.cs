using Foundation;
using UIKit;
using RobotGameShared;

namespace RobotGame {
    [Register("AppDelegate")]
    class Program : UIApplicationDelegate {
        private static Game game;

        internal static void RunGame() {
            game = new Game();
            game.Run();
        }

        static void Main(string[] args) {
            UIApplication.Main(args, null, "AppDelegate");
        }

        public override void FinishedLaunching(UIApplication app) {
            RunGame();
        }
    }
}
