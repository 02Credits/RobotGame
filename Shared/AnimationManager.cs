using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace RobotGameShared {
    public class AnimationManager : IContentLoadable {
        List<ReloadingTexture> animationCurveTextures = new List<ReloadingTexture>();
        Dictionary<string, double[]> animationCurves = new Dictionary<string, double[]>();

        Func<string, Action<Texture2D>, ReloadingTexture> reloadingTextureFactory;

        public AnimationManager(Func<string, Action<Texture2D>, ReloadingTexture> reloadingTextureFactory) {
            this.reloadingTextureFactory = reloadingTextureFactory;
        }

        public double Sample(string curveName, double progress) {
            double[] curve = animationCurves[curveName];
            double index = progress * curve.Length;
            double remainder = index - (int)index;

            double floorValue = curve[(int)Math.Floor(Math.Max(index, 0)) % curve.Length];
            double ceilValue = curve[(int)Math.Ceiling(Math.Max(index, 0)) % curve.Length];

            return (1.0 - remainder) * floorValue + remainder * ceilValue;
        }

        public int Interpolate(string curveName, double progress, int from, int to) {
            int diff = to - from;
            return (int)(Sample(curveName, progress) * diff) + from;
        }

        public double Interpolate(string curveName, double progress, double from, double to) {
            double diff = to - from;
            return Sample(curveName, progress) * diff + from;
        }

        public Vector2 Interpolate(string curveName, double progress, Vector2 from, Vector2 to) {
            Vector2 diff = to - from;
            return (float)Sample(curveName, progress) * diff + from;
        }

        public void LoadContent(ContentManager content) {
            LoadCurve("Test", content);
        }

        private void LoadCurve(string textureName, ContentManager content) {
            var reloadingTexture = reloadingTextureFactory($"AnimationCurves/{textureName}", texture => {
                animationCurves[textureName] = BuildCurve(texture);
            });
            animationCurveTextures.Add(reloadingTexture);
        }

        private double[] BuildCurve(Texture2D texture) {
            var pixels = new Color[texture.Width * texture.Height];
            texture.GetData(pixels);
            Color GetPixel(int x, int y) => pixels[y * texture.Width + x];

            var curve = new double[texture.Width];

            for (var x = 0; x < texture.Width; x++) {
                var totalY = 0.0;
                var yCount = 0.0;

                for (var y = 0; y < texture.Height; y++) {
                    var pixelColor = GetPixel(x, y);
                    if (pixelColor.A != 0 && pixelColor != Color.White) {
                        totalY += y;
                        yCount++;
                    }
                }
                curve[x] = 1.0 - totalY / yCount / texture.Height;
            }

            return curve;
        }
    }
}
