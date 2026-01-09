using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using ImageMagick;
using SFML.Graphics;
using SFML.System;

namespace vimage.Display
{
    public abstract record TextureType;

    public record SingleTexture(Texture Texture) : TextureType;

    public record SplitTexture(DisplayObject DisplayObject) : TextureType;

    /// <summary>
    /// Graphics Manager.
    /// Loads and stores Textures and AnimatedImageDatas.
    /// /// </summary>
    internal class Graphics
    {
        private static readonly OrderedDictionary<string, TextureType> Textures = [];
        private static readonly OrderedDictionary<string, AnimatedImageData> AnimatedImageDatas =
        [];

        public static uint MaxTextures = 40;
        public static uint MaxAnimations = 8;
        public static uint TextureMaxSize = Texture.MaximumSize;

        public static SFML.ObjectBase? GetImage(
            string fileName,
            MagickReadSettings? settings = null
        )
        {
            TextureType? texture = null;

            lock (Textures)
            {
                int index = Textures.IndexOf(fileName);
                if (index >= 0)
                {
                    // Texture Already Exists
                    // move it to the end of the array and return it
                    var (name, existingTexture) = Textures.GetAt(index);
                    Textures.RemoveAt(index);
                    Textures.Add(name, existingTexture);
                    texture = existingTexture;
                }
            }

            if (texture == null)
            {
                // New Texture
                try
                {
                    texture = GetTexture(fileName, settings);
                }
                catch (Exception) { }
            }

            if (texture is SingleTexture tex)
                return new Sprite(tex.Texture);
            else if (texture is SplitTexture splitTexture)
                return splitTexture.DisplayObject;
            return null;
        }

        public static TextureType? GetTexture(
            string fileName,
            MagickReadSettings? settings = null,
            bool cache = true
        )
        {
            using var image = GetMagickImage(fileName, settings);
            if (image is null)
                return null;

            TextureType output;

            if (image.Width > TextureMaxSize || image.Height > TextureMaxSize)
            {
                // Image is too large -> split it up into multiple textures
                output = new SplitTexture(GetLargeTexture(image, TextureMaxSize));
            }
            else
            {
                using var pixels = image.GetPixels();
                var bytes = pixels.ToByteArray(PixelMapping.RGBA);
                var texture = new Texture(image.Width, image.Height);
                texture.Update(bytes);
                output = new SingleTexture(texture);
            }

            if (!cache)
                return output;

            lock (Textures)
            {
                Textures.Add(fileName, output);
                // Limit amount of textures in memory
                if (Textures.Count > MaxTextures)
                    RemoveTexture();
            }
            return output;
        }

        private static DisplayObject GetLargeTexture(MagickImage image, uint sectionSize)
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
                    pos.X += w;
                }
                pos.Y += h;
                pos.X = 0;
            }

            largeTexture.Texture.Size = new Vector2u(image.Width, image.Height);
            return largeTexture;
        }

        /// <summary>Loads an image into memory but doesn't set it as the displayed image.</summary>
        public static bool PreloadImage(string fileName)
        {
            if (!File.Exists(fileName))
                return false;

            if (Utils.ImageViewerUtils.IsAnimatedImage(fileName))
            {
                lock (AnimatedImageDatas)
                {
                    if (AnimatedImageDatas.ContainsKey(fileName))
                        return true;
                }
                return GetAnimatedImageData(fileName) is not null;
            }

            lock (Textures)
            {
                if (Textures.ContainsKey(fileName))
                    return true;
            }
            return GetTexture(fileName) is not null;
        }

        public static MagickImage? GetMagickImage(
            string fileName,
            MagickReadSettings? settings = null
        )
        {
            var info = Utils.ImageViewerUtils.GetMagickImageInfo(fileName);
            if (info is not null && info.Format == MagickFormat.Ico)
                return GetMagickImageIco(fileName);

            var image = settings is null
                ? new MagickImage(
                    fileName,
                    Utils.ImageViewerUtils.GetDefaultMagickReadSettings(s =>
                        s.BackgroundColor = MagickColors.None
                    )
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
                var index = AnimatedImageDatas.IndexOf(fileName);

                if (index >= 0)
                {
                    // AnimatedImageData Already Exists
                    // move it to the end of the array and return it
                    var (name, data) = AnimatedImageDatas.GetAt(index);
                    AnimatedImageDatas.RemoveAt(index);
                    AnimatedImageDatas.Add(name, data);
                    return data;
                }
                else
                {
                    // New AnimatedImageData
                    var data = new AnimatedImageData();

                    // Store AnimatedImageData
                    AnimatedImageDatas.Add(fileName, data);

                    // Limit amount of animations in memory
                    if (AnimatedImageDatas.Count > MaxAnimations)
                        RemoveAnimatedImage();

                    // Get Frames
                    var info = Utils.ImageViewerUtils.GetMagickImageInfo(fileName);
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
        /// Called from `-clearMemory` command or by using reset image with `ClearMemoryOnResetImage` enabled.
        /// </summary>
        /// <param name="image">The currently viewed image.</param>
        /// <param name="file">The currently viewed image filepath.</param>
        public static void ClearMemory(object? image, string file = "")
        {
            // Remove all AnimatedImages (except the one that's currently being viewed)
            lock (AnimatedImageDatas)
            {
                int s = image is AnimatedImage ? 1 : 0;
                while (AnimatedImageDatas.Count > s)
                    RemoveAnimatedImage();
            }

            lock (Textures)
            {
                if (image is AnimatedImage)
                {
                    // Remove all Textures
                    while (Textures.Count > 0)
                        RemoveTexture();
                }
                else
                {
                    // Remove all Textures (except ones being used by current image)
                    int a = 0;
                    while (Textures.Count > 1)
                    {
                        if (Textures.GetAt(0).Key == file)
                            a++;
                        RemoveTexture(a);
                    }
                }
            }

            // Force garbage collection
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            GC.WaitForPendingFinalizers();
        }

        public static void RemoveTexture(int index = 0)
        {
            var (_, texture) = Textures.GetAt(index);

            if (texture is SplitTexture splitTexture)
            {
                List<Sprite> sprites = [];
                for (int i = splitTexture.DisplayObject.NumChildren - 1; i >= 0; i--)
                {
                    var child = splitTexture.DisplayObject.GetChildAt(i);
                    if (child is not Sprite sprite)
                        continue;
                    sprites.Add(sprite);
                }
                splitTexture.DisplayObject.Clear();
                splitTexture.DisplayObject.Dispose();
                foreach (var sprite in sprites)
                {
                    sprite.Texture.Dispose();
                    sprite.Dispose();
                }
            }
            else if (texture is SingleTexture singleTexture)
            {
                singleTexture.Texture.Dispose();
            }
            Textures.RemoveAt(index);
        }

        public static void RemoveAnimatedImage(int index = 0)
        {
            var (_, data) = AnimatedImageDatas.GetAt(index);
            if (data != null)
            {
                data.CancelLoading = true;
                for (int i = 0; i < data.Frames.Length; i++)
                    data.Frames[i]?.Dispose();
            }
            AnimatedImageDatas.RemoveAt(index);
        }

        public static void RemoveFileFromMemory(string fileName)
        {
            var index = Textures.IndexOf(fileName);
            if (index != -1)
            {
                RemoveTexture(index);
                return;
            }
            index = AnimatedImageDatas.IndexOf(fileName);
            if (index != -1)
                RemoveAnimatedImage(index);
        }
    }

    internal class LoadingAnimatedImageFromMagick(string fileName, AnimatedImageData data)
    {
        private readonly string FileName = fileName;
        private readonly AnimatedImageData Data = data;

        public void LoadFrames()
        {
            var settings = new MagickReadSettings { };

            var info = Utils.ImageViewerUtils.GetMagickImageInfo(FileName);
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
            collection.Coalesce(); // TODO: Find alternative that doesn't use as much resources
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
