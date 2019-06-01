using System;
using System.Collections.Generic;
using System.Text;

namespace RobotGameShared.UI {
    public class AnonymousDisposable : IDisposable {
        private Action onDisposed;

        public AnonymousDisposable(Action onDisposed) {
            this.onDisposed = onDisposed;
        }

        public void Dispose() {
            onDisposed?.Invoke();
            onDisposed = null;
        }
    }
}
