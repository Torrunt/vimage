using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using ImageMagick;
using SFML.Graphics;
using SFML.System;

namespace vimage
{
    /// <summary>
    /// Graphics Manager.
    /// Loads and stores Textures and AnimatedImageDatas.
    /// </summary>
    internal class Graphics
    {
        private static readonly List<Texture> Textures = [];
        private static readonly List<string> TextureFileNames = [];

        private static readonly List<AnimatedImageData> AnimatedImageDatas = [];
        private static readonly List<string> AnimatedImageDataFileNames = [];

        private static readonly List<DisplayObject> SplitTextures = [];
        private static readonly List<string> SplitTextureFileNames = [];

        public static uint MAX_TEXTURES = 80;
        public static uint MAX_ANIMATIONS = 8;
        public static uint TextureMaxSize = Texture.MaximumSize;

        public static object? GetImage(string fileName, MagickReadSettings? settings = null)
        {
            int index = TextureFileNames.IndexOf(fileName);

            if (index >= 0)
            {
                // Texture Already Exists
                // move it to the end of the array and return it
                var texture = Textures[index];
                var name = TextureFileNames[index];

                Textures.RemoveAt(index);
                TextureFileNames.RemoveAt(index);
                Textures.Add(texture);
                TextureFileNames.Add(name);

                return new Sprite(Textures[^1]);
            }
            else
            {
                // New Texture
                try
                {
                    var texture = GetTexture(fileName, settings);
                    if (texture == null)
                        return null;
                    if (texture is Texture tex)
                        return new Sprite(new Texture(tex));
                    else if (texture is DisplayObject displayObject)
                        return displayObject;
                }
                catch (Exception) { }
            }

            return null;
        }

        public static object? GetTexture(
            string fileName,
            MagickReadSettings? settings = null,
            bool cache = true
        )
        {
            using var image = GetMagickImage(fileName, settings);
            if (image is null)
                return null;

            if (image.Width > TextureMaxSize || image.Height > TextureMaxSize)
            {
                return GetLargeTexture(image, TextureMaxSize, cache ? fileName : null);
            }
            else
            {
                using var pixels = image.GetPixels();
                var bytes = pixels.ToByteArray(PixelMapping.RGBA);
                var texture = new Texture(image.Width, image.Height);
                texture.Update(bytes);
                if (cache)
                {
                    Textures.Add(texture);
                    TextureFileNames.Add(fileName);
                }

                return texture;
            }
        }

        private static DisplayObject GetLargeTexture(
            MagickImage image,
            uint sectionSize,
            string? fileName = null
        )
        {
            var amount = new Vector2u(
                (uint)Math.Ceiling((float)image.Width / sectionSize),
                (uint)Math.Ceiling((float)image.Height / sectionSize)
            );
            var currentSize = new Vector2u(image.Width, image.Height);
            var pos = new Vector2u();

            var largeTexture = new DisplayObject();

            using var pixels = image.GetPixels();

            for (int iy = 0; iy < amount.Y; iy++)
            {
                var h = Math.Min(currentSize.Y, sectionSize);
                currentSize.Y -= h;
                currentSize.X = image.Width;

                for (int ix = 0; ix < amount.X; ix++)
                {
                    var w = Math.Min(currentSize.X, sectionSize);
                    currentSize.X -= w;

                    var texture = new Texture(w, h);
                    var bytes = pixels.ToByteArray((int)pos.X, (int)pos.Y, w, h, PixelMapping.RGBA);
                    texture.Update(bytes);
                    var sprite = new Sprite(texture) { Position = new Vector2f(pos.X, pos.Y) };
                    largeTexture.AddChild(sprite);

                    if (fileName is not null)
                    {
                        Textures.Add(texture);
                        TextureFileNames.Add(
                            fileName + "_" + ix.ToString("00") + "_" + iy.ToString("00") + "^"
                        );
                    }

                    pos.X += w;
                }
                pos.Y += h;
                pos.X = 0;
            }

            largeTexture.Texture.Size = new Vector2u(image.Width, image.Height);
            if (fileName is not null)
            {
                SplitTextures.Add(largeTexture);
                SplitTextureFileNames.Add(fileName);
            }

            return largeTexture;
        }

        public static MagickImage? GetMagickImage(
            string fileName,
            MagickReadSettings? settings = null
        )
        {
            var info = MagickFormatInfo.Create(fileName);
            if (info is not null && info.Format == MagickFormat.Ico)
                return GetMagickImageIco(fileName);

            var image = settings is null
                ? new MagickImage(
                    fileName,
                    new MagickReadSettings { BackgroundColor = MagickColors.None }
                )
                : new MagickImage(fileName, settings);
            if (image is null)
                return null;
            image.Format = MagickFormat.Rgba;
            return image;
        }

        /// <summary>Gets the highest resolution image in the .ico</summary>
        private static MagickImage? GetMagickImageIco(string fileName)
        {
            using var images = new MagickImageCollection(fileName);
            var best = images.OrderByDescending(i => i.Width * i.Height).First();
            return new MagickImage(best);
        }

        /// <param name="filename">Animated Image (ie: animated gif).</param>
        public static AnimatedImage GetAnimatedImage(string fileName)
        {
            return new AnimatedImage(GetAnimatedImageData(fileName));
        }

        /// <param name="filename">Animated Image (ie: animated gif).</param>
        public static AnimatedImageData GetAnimatedImageData(string fileName)
        {
            lock (AnimatedImageDatas)
            {
                int index = AnimatedImageDataFileNames.IndexOf(fileName);

                if (index >= 0)
                {
                    // AnimatedImageData Already Exists
                    // move it to the end of the array and return it
                    var data = AnimatedImageDatas[index];
                    string name = AnimatedImageDataFileNames[index];

                    AnimatedImageDatas.RemoveAt(index);
                    AnimatedImageDataFileNames.RemoveAt(index);
                    AnimatedImageDatas.Add(data);
                    AnimatedImageDataFileNames.Add(name);

                    return AnimatedImageDatas[^1];
                }
                else
                {
                    // New AnimatedImageData
                    var data = new AnimatedImageData();

                    // Store AnimatedImageData
                    AnimatedImageDatas.Add(data);
                    AnimatedImageDataFileNames.Add(fileName);

                    // Limit amount of Animations in Memory
                    if (AnimatedImageDatas.Count > MAX_ANIMATIONS)
                        RemoveAnimatedImage();

                    // Get Frames
                    var info = MagickFormatInfo.Create(fileName);
                    if (info is not null && info.Format == MagickFormat.Gif)
                    {
                        // Use System.Drawing and OctreeQuantizer (faster and uses less memory)
                        var loadingAnimatedImage = new LoadingAnimatedImage(fileName, data);
                        var loadFramesThread = new Thread(
                            new ThreadStart(loadingAnimatedImage.LoadFrames)
                        )
                        {
                            Name = "AnimationLoadThread - " + fileName,
                            IsBackground = true,
                        };
                        loadFramesThread.Start();
                    }
                    else
                    {
                        // Use ImageMagick
                        var loadingAnimatedImage = new LoadingAnimatedImageFromMagick(
                            fileName,
                            data
                        );
                        var loadFramesThread = new Thread(
                            new ThreadStart(loadingAnimatedImage.LoadFrames)
                        )
                        {
                            Name = "AnimationLoadThread - " + fileName,
                            IsBackground = true,
                        };
                        loadFramesThread.Start();
                    }

                    // Wait for at least one frame to be loaded
                    while (data.Frames == null || data.Frames.Length <= 0 || data.Frames[0] == null)
                    {
                        Thread.Sleep(1);
                    }

                    return data;
                }
            }
        }

        /// <summary>
        /// Clears all images/textures from memory (except the currently viewed image).
        /// Called from `-clearMemory` command or by using reset image with `Setting_ClearMemoryOnResetImage` enabled.
        /// </summary>
        /// <param name="image">The currently viewed image.</param>
        /// <param name="file">The currently viewed image filepath.</param>
        public static void ClearMemory(dynamic image, string file = "")
        {
            // Remove all AnimatedImages (except the one that's currently being viewed)
            int s = image is AnimatedImage ? 1 : 0;
            int a = 0;
            while (AnimatedImageDatas.Count > s)
            {
                if (
                    s == 1
                    && image is AnimatedImage animatedImage
                    && animatedImage.Data == AnimatedImageDatas[a]
                )
                    a++;
                RemoveAnimatedImage(a);
            }

            if (image is AnimatedImage)
            {
                // Remove all Textures
                while (TextureFileNames.Count > 0)
                    RemoveTexture();
            }
            else
            {
                // Remove all Textures (except ones being used by current image)
                if (file == "")
                    return;

                s = 0;
                a = 0;
                if (SplitTextureFileNames.Contains(file))
                {
                    for (int i = 0; i < TextureFileNames.Count; i++)
                    {
                        if (TextureFileNames[i].StartsWith(file))
                            s++;
                        else if (s > 0)
                            break;
                    }
                    while (TextureFileNames.Count > s)
                    {
                        if (TextureFileNames[a].StartsWith(file))
                            a += s;
                        RemoveTexture(a);
                    }
                }
                else
                {
                    while (TextureFileNames.Count > 1)
                    {
                        if (TextureFileNames[a] == file)
                            a++;
                        RemoveTexture(a);
                    }
                }
            }

            // Force garbage collection
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            GC.WaitForPendingFinalizers();
        }

        public static void RemoveTexture(int t = 0)
        {
            if (TextureFileNames[t].IndexOf('^') == TextureFileNames[t].Length - 1)
            {
                // if part of split texture - remove all parts
                string name = TextureFileNames[t][..^7];

                int i;
                for (i = t + 1; i < TextureFileNames.Count; i++)
                {
                    if (!TextureFileNames[i].StartsWith(name))
                        break;
                }
                for (int d = t; d < i; d++)
                {
                    Textures[t]?.Dispose();
                    Textures.RemoveAt(t);
                    TextureFileNames.RemoveAt(t);
                }

                int splitIndex =
                    SplitTextureFileNames.Count == 0 ? -1 : SplitTextureFileNames.IndexOf(name);
                if (splitIndex != -1)
                {
                    SplitTextures.RemoveAt(splitIndex);
                    SplitTextureFileNames.RemoveAt(splitIndex);
                }
            }
            else
            {
                Textures[t]?.Dispose();
                Textures.RemoveAt(t);
                TextureFileNames.RemoveAt(t);
            }
        }

        public static void RemoveAnimatedImage(int a = 0)
        {
            AnimatedImageDatas[a].CancelLoading = true;
            for (int i = 0; i < AnimatedImageDatas[a].Frames.Length; i++)
                AnimatedImageDatas[a]?.Frames[i]?.Dispose();
            AnimatedImageDatas.RemoveAt(a);
            AnimatedImageDataFileNames.RemoveAt(a);
        }
    }

    internal class LoadingAnimatedImageFromMagick(string fileName, AnimatedImageData data)
    {
        private readonly string FileName = fileName;
        private readonly AnimatedImageData Data = data;

        public void LoadFrames()
        {
            var settings = new MagickReadSettings { };

            var info = MagickFormatInfo.Create(FileName);
            if (
                info is not null
                && (info.Format == MagickFormat.APng || info.Format == MagickFormat.Png)
            )
                settings.Format = MagickFormat.APng;

            using var collection = new MagickImageCollection();
            collection.Ping(FileName, settings);

            Data.FrameCount = collection.Count;
            Data.Frames = new Texture[Data.FrameCount];
            Data.FrameDelays = new int[Data.FrameCount];

            int defaultFrameDelay = AnimatedImage.DEFAULT_FRAME_DELAY;

            // Load first frame (so it can be shown as soon as possible)
            ReadAndLoadFrame(0);

            // Process the rest
            collection.Read(FileName, settings);
            collection.Coalesce(); // FIXME: Need alternative that doesn't use as much resources
            for (int i = 0; i < Data.FrameCount; i++)
            {
                if (Data.CancelLoading)
                    return;
                using var frame = collection[i];
                LoadFrame(frame, i);
            }

            Data.FullyLoaded = true;
        }

        public void LoadFrame(IMagickImage<byte> frame, int index)
        {
            var delay = frame.AnimationDelay * 10;
            Data.FrameDelays[index] = delay > 0 ? (int)delay : AnimatedImage.DEFAULT_FRAME_DELAY;

            using var pixels = frame.GetPixelsUnsafe();
            var bytes = pixels.ToByteArray(PixelMapping.RGBA);
            var texture = new Texture(frame.Width, frame.Height);
            texture.Update(bytes);

            Data.Frames[index] = texture;
            texture.Smooth = Data.Smooth;
            if (Data.Mipmap)
                texture.GenerateMipmap();
        }

        public void ReadAndLoadFrame(int index)
        {
            using var frame = new MagickImage(
                FileName,
                new MagickReadSettings
                {
                    FrameIndex = (uint)index,
                    FrameCount = 1,
                    BackgroundColor = MagickColors.None,
                }
            );
            LoadFrame(frame, index);
        }
    }

    internal class LoadingAnimatedImage(string fileName, AnimatedImageData data)
    {
        private readonly string FileName = fileName;
        private ImageManipulation.OctreeQuantizer? Quantizer;
        private readonly AnimatedImageData Data = data;

        public void LoadFrames()
        {
            using var image = System.Drawing.Image.FromFile(FileName);

            // Get Frame Count
            var frameDimension = new System.Drawing.Imaging.FrameDimension(
                image.FrameDimensionsList[0]
            );
            Data.FrameCount = image.GetFrameCount(frameDimension);
            Data.Frames = new Texture[Data.FrameCount];
            Data.FrameDelays = new int[Data.FrameCount];

            // Get Frame Delays
            byte[]? frameDelays = null;
            try
            {
                var frameDelaysItem = image.GetPropertyItem(0x5100);
                if (frameDelaysItem is not null)
                {
                    frameDelays = frameDelaysItem.Value;
                    if (
                        frameDelays is null
                        || frameDelays.Length == 0
                        || (frameDelays[0] == 0 && frameDelays.All(d => d == 0))
                    )
                        frameDelays = null;
                }
            }
            catch { }
            int defaultFrameDelay = AnimatedImage.DEFAULT_FRAME_DELAY;
            if (frameDelays != null && frameDelays.Length > 1)
                defaultFrameDelay = (frameDelays[0] + frameDelays[1] * 256) * 10;

            for (int i = 0; i < Data.FrameCount; i++)
            {
                if (Data.CancelLoading)
                    return;

                int fd = i * 4;
                Data.FrameDelays[i] =
                    frameDelays != null && frameDelays.Length > fd
                        ? (frameDelays[fd] + frameDelays[fd + 1] * 256) * 10
                        : defaultFrameDelay;

                _ = image.SelectActiveFrame(frameDimension, i);
                Quantizer = new ImageManipulation.OctreeQuantizer(255, 8);

                using var quantized = Quantizer.Quantize(image);
                using var stream = new MemoryStream();
                quantized.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                var texture = new Texture(stream);

                Data.Frames[i] = texture;
                texture.Smooth = Data.Smooth;
                if (Data.Mipmap)
                    texture.GenerateMipmap();
            }
            Data.FullyLoaded = true;
        }
    }
}
