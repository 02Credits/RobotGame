using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RobotGameShared {
    public class ResourceReloader : IContentLoadable, IUpdateable {
        Dictionary<string, ReloadingResource> reloadingResources = new Dictionary<string, ReloadingResource>();
        ConcurrentQueue<string> resourcesToReload = new ConcurrentQueue<string>();

        public int UpdateOrder => 1;

        public void LoadContent(ContentManager content) {
            foreach (ReloadingResource resource in reloadingResources.Values) {
#if DEBUG && !IOS
                resource.DebugReload();
#else
                resource.ProductionLoad(content);
#endif
            }
        }

        public void Update(GameTime gameTime) {
            while (resourcesToReload.TryDequeue(out var resourcePath)) {
                reloadingResources[resourcePath].DebugReload();
            }
        }

        public void TriggerReload(string resourcePath) => resourcesToReload.Enqueue(resourcePath);

        public void Register(string resourcePath, ReloadingResource resource) => reloadingResources[resourcePath] = resource;
        public void Unregister(string resourcePath) => reloadingResources.Remove(resourcePath);
    }

    public abstract class ReloadingResource : IDisposable {
        ResourceReloader resourceReloader;

        // Name of the resource to use in the content directory
        protected string resourceName;
        // Path under the Shared/Content directory to watch
        protected string debugPath;

        FileSystemWatcher watcher;

        public ReloadingResource(ResourceReloader resourceReloader, string resourceName) {
#if DEBUG && !IOS
            this.resourceReloader = resourceReloader;
            this.resourceName = resourceName;

            watcher = new FileSystemWatcher();
            debugPath = $"../../../../Shared/Content/{resourceName}";

            watcher.Path = Path.GetDirectoryName(debugPath);
            watcher.Filter = Path.GetFileName(debugPath) + ".*";
            watcher.Changed += (_, __) => {
                resourceReloader.TriggerReload(resourceName);
            };
            watcher.EnableRaisingEvents = true;

            resourceReloader.Register(resourceName, this);
#endif
        }

        public abstract void DebugReload();
        public abstract void ProductionLoad(ContentManager content);

        public void Dispose() {
#if DEBUG && !IOS
            resourceReloader.Unregister(resourceName);
            watcher.Dispose();
#endif
        }
    }

    public class ReloadingTexture : ReloadingResource {
        Action<Texture2D> loadAction;
        GraphicsDevice graphicsDevice;

        public Texture2D LoadedTexture { get; private set; }

        public ReloadingTexture(GraphicsDevice graphicsDevice, ResourceReloader resourceReloader, string textureName, Action<Texture2D> loadAction = null) 
            : base(resourceReloader, textureName) {
            this.graphicsDevice = graphicsDevice;
            this.loadAction = loadAction;
        }

        public override void DebugReload() {
            using (FileStream textureStream = new FileStream(debugPath + ".png", FileMode.Open)) {
                LoadedTexture = Texture2D.FromStream(graphicsDevice, textureStream);
                loadAction?.Invoke(LoadedTexture);
            }
        }

        public override void ProductionLoad(ContentManager content) {
            LoadedTexture = content.Load<Texture2D>(resourceName);
            loadAction?.Invoke(LoadedTexture);
        }
    }
}
