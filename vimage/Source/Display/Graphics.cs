using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using DevILSharp;
using SFML.Graphics;
using SFML.System;
using SkiaSharp;
using Svg.Skia;

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
        public static int TextureMaxSize = (int)Texture.MaximumSize;

        public static bool UseDevil = false;

        public static void InitDevIL()
        {
            try
            {
                IL.Init();
                UseDevil = true;
            }
            catch (DllNotFoundException)
            {
                if (OperatingSystem.IsWindowsVersionAtLeast(6, 1))
                {
                    System.Windows.Forms.MessageBox.Show(
                        "vimage failed to find DevIL.dll.\nIf problem persists, try disabling DevIL in the settings.",
                        "vimage - DevIL.dll not found"
                    );
                }
            }
        }

        public static dynamic GetTexture(string fileName)
        {
            int index = TextureFileNames.IndexOf(fileName);
            int splitTextureIndex =
                SplitTextureFileNames.Count == 0 ? -1 : SplitTextureFileNames.IndexOf(fileName);

            if (index >= 0)
            {
                // Texture Already Exists
                // move it to the end of the array and return it
                Texture texture = Textures[index];
                string name = TextureFileNames[index];

                Textures.RemoveAt(index);
                TextureFileNames.RemoveAt(index);
                Textures.Add(texture);
                TextureFileNames.Add(name);

                return Textures[Textures.Count - 1];
            }
            else if (splitTextureIndex >= 0)
            {
                // Texture Already Exists (as split texture)
                return SplitTextures[splitTextureIndex];
            }
            else
            {
                // New Texture
                Texture texture = null;
                DisplayObject textureLarge = null;

                using (FileStream fileStream = File.OpenRead(fileName))
                {
                    if (UseDevil)
                    {
                        // Load image via DevIL
                        int imageID = IL.GenImage();
                        IL.BindImage(imageID);

                        _ = IL.Enable(EnableCap.AbsoluteOrigin);
                        IL.RegisterOrigin(OriginMode.UpperLeft); // IL.SetOriginLocation(DevIL.OriginLocation.UpperLeft);

                        bool loaded = IL.LoadStream(fileStream);

                        if (loaded)
                        {
                            int width = IL.GetInteger(IntName.ImageWidth);
                            int height = IL.GetInteger(IntName.ImageHeight);
                            if (width > TextureMaxSize || height > TextureMaxSize)
                            {
                                // Large Image split-up into multiple textures
                                // (image is larger than GPU's max texture size)
                                textureLarge = GetLargeTextureFromBoundImage(
                                    TextureMaxSize / 2,
                                    fileName
                                );
                            }
                            else
                            {
                                // Single Texture
                                texture = GetTextureFromBoundImage();
                                if (texture == null)
                                    return null;

                                Textures.Add(texture);
                                TextureFileNames.Add(fileName);
                            }
                        }
                        IL.DeleteImage(imageID);
                    }
                    else
                    {
                        // Load image via SFML
                        try
                        {
                            using (var image = new SFML.Graphics.Image(fileStream))
                            {
                                Vector2u imageSize = image.Size;

                                if (imageSize.X > TextureMaxSize || imageSize.Y > TextureMaxSize)
                                {
                                    // Large Image split-up into multiple textures
                                    textureLarge = GetLargeTextureFromSFMLImage(
                                        TextureMaxSize,
                                        image,
                                        fileName
                                    );
                                }
                                else
                                {
                                    // Single Texture
                                    texture = GetTextureFromSFMLImage(image);
                                    Textures.Add(texture);
                                    TextureFileNames.Add(fileName);
                                }
                            }
                        }
                        catch (SFML.LoadingFailedException)
                        {
                            if (OperatingSystem.IsWindowsVersionAtLeast(6, 1))
                            {
                                System.Windows.Forms.MessageBox.Show(
                                    "Failed to load image:\n"
                                        + fileName
                                        + ".\n\nTry changing \"Use DevIL\" on in the settings to help with this issue.",
                                    "vimage - SFML Image Loading Failed"
                                );
                            }
                            return null;
                        }
                    }
                }

                // Limit amount of Textures in Memory
                if (Textures.Count > MAX_TEXTURES)
                    RemoveTexture();

                return texture ?? (dynamic)textureLarge;
            }
        }

        private static Texture GetTextureFromBoundImage()
        {
            bool success = IL.ConvertImage(ChannelFormat.RGBA, ChannelType.UnsignedByte);
            if (!success)
                return null;

            int width = IL.GetInteger(IntName.ImageWidth);
            int height = IL.GetInteger(IntName.ImageHeight);
            int bytesPerPixel = 4;
            int rowBytes = width * bytesPerPixel;
            int length = width * height * bytesPerPixel;
            nint dataPtr = IL.GetData();
            byte[] pixels = new byte[length];
            Marshal.Copy(dataPtr, pixels, 0, length);

            // Flip pixels since DevIL and SFML use different coordinates
            byte[] flippedPixels = new byte[pixels.Length];
            for (int y = 0; y < height; y++)
            {
                System.Buffer.BlockCopy(
                    pixels,
                    y * rowBytes,
                    flippedPixels,
                    (height - 1 - y) * rowBytes,
                    rowBytes
                );
            }

            var texture = new Texture((uint)width, (uint)height);
            texture.Update(flippedPixels);
            return texture;
        }

        private static DisplayObject GetLargeTextureFromBoundImage(
            int sectionSize,
            string fileName = ""
        )
        {
            bool success = IL.ConvertImage(ChannelFormat.RGBA, ChannelType.UnsignedByte);
            if (!success)
                return null;

            var largeTexture = new DisplayObject();

            int width = IL.GetInteger(IntName.ImageWidth);
            int height = IL.GetInteger(IntName.ImageHeight);
            var size = new Vector2i(width, height);
            var amount = new Vector2u(
                (uint)Math.Ceiling(size.X / (float)sectionSize),
                (uint)Math.Ceiling(size.Y / (float)sectionSize)
            );

            var currentSize = new Vector2i(size.X, size.Y);
            var pos = new Vector2i();
            int bytesPerPixel = 4;

            int maxSectionBytes = sectionSize * sectionSize * bytesPerPixel;
            byte[] pixelBuffer = new byte[maxSectionBytes];
            byte[] flippedBuffer = new byte[maxSectionBytes];

            for (int iy = 0; iy < amount.Y; iy++)
            {
                int h = Math.Min(currentSize.Y, sectionSize);
                currentSize.Y -= h;
                currentSize.X = size.X;

                for (int ix = 0; ix < amount.X; ix++)
                {
                    int w = Math.Min(currentSize.X, sectionSize);
                    currentSize.X -= w;

                    int sectionBytes = w * h * bytesPerPixel;

                    // Copy pixels from DevIL into reusable buffer
                    var partPtr = Marshal.AllocHGlobal(sectionBytes);
                    _ = IL.CopyPixels(
                        pos.X,
                        pos.Y,
                        0,
                        w,
                        h,
                        1,
                        ChannelFormat.RGBA,
                        ChannelType.UnsignedByte,
                        partPtr
                    );
                    Marshal.Copy(partPtr, pixelBuffer, 0, sectionBytes);
                    Marshal.FreeHGlobal(partPtr);

                    // Flip pixels since DevIL and SFML use different coordinates
                    int rowBytes = w * bytesPerPixel;
                    for (int y = 0; y < h; y++)
                    {
                        System.Buffer.BlockCopy(
                            pixelBuffer,
                            y * rowBytes,
                            flippedBuffer,
                            (h - 1 - y) * rowBytes,
                            rowBytes
                        );
                    }

                    var texture = new Texture((uint)w, (uint)h);
                    texture.Update(flippedBuffer, (uint)w, (uint)h, 0, 0);

                    var sprite = new Sprite(texture)
                    {
                        Position = new Vector2f(pos.X, size.Y - pos.Y - h),
                    };
                    largeTexture.AddChild(sprite);

                    if (fileName != "")
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

            largeTexture.Texture.Size = new Vector2u((uint)size.X, (uint)size.Y);
            SplitTextures.Add(largeTexture);
            SplitTextureFileNames.Add(fileName);

            return largeTexture;
        }

        private static Texture GetTextureFromSFMLImage(
            SFML.Graphics.Image image,
            IntRect area = default
        )
        {
            Vector2u imageSize = image.Size;
            byte[] bytes = image.Pixels;

            if (area == default || (area.Width >= imageSize.X && area.Height >= imageSize.Y))
                return new Texture(bytes);

            const int blockSize = 4;
            var crop = new byte[area.Width * area.Height * blockSize];
            for (var line = 0; line <= area.Height - 1; line++)
            {
                var sourceIndex = ((area.Top + line) * imageSize.X + area.Left) * blockSize;
                var destinationIndex = line * area.Width * blockSize;

                Array.Copy(bytes, sourceIndex, crop, destinationIndex, area.Width * blockSize);
            }
            return new Texture(crop);
        }

        private static DisplayObject GetLargeTextureFromSFMLImage(
            int sectionSize,
            SFML.Graphics.Image image,
            string fileName = ""
        )
        {
            var imageSize = image.Size;
            var amount = new Vector2u(
                (uint)Math.Ceiling((float)imageSize.X / sectionSize),
                (uint)Math.Ceiling((float)imageSize.Y / sectionSize)
            );
            var currentSize = new Vector2i((int)imageSize.X, (int)imageSize.Y);
            var pos = new Vector2i();

            var largeTexture = new DisplayObject();

            for (int iy = 0; iy < amount.Y; iy++)
            {
                int h = Math.Min(currentSize.Y, sectionSize);
                currentSize.Y -= h;
                currentSize.X = (int)imageSize.X;

                for (int ix = 0; ix < amount.X; ix++)
                {
                    int w = Math.Min(currentSize.X, sectionSize);
                    currentSize.X -= w;

                    var texture = GetTextureFromSFMLImage(image, new IntRect(pos.X, pos.Y, w, h));
                    var sprite = new Sprite(texture) { Position = new Vector2f(pos.X, pos.Y) };
                    largeTexture.AddChild(sprite);

                    if (fileName != "")
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

            largeTexture.Texture.Size = new Vector2u(imageSize.X, imageSize.Y);
            SplitTextures.Add(largeTexture);
            SplitTextureFileNames.Add(fileName);

            return largeTexture;
        }

        public static Sprite GetSpriteFromIcon(string fileName)
        {
            int index = TextureFileNames.IndexOf(fileName);

            if (index >= 0)
            {
                // Texture Already Exists
                // move it to the end of the array and return it
                var texture = Textures[index];
                string name = TextureFileNames[index];

                Textures.RemoveAt(index);
                TextureFileNames.RemoveAt(index);
                Textures.Add(texture);
                TextureFileNames.Add(name);

                return new Sprite(Textures[Textures.Count - 1]);
            }
            else
            {
                // New Texture (from .ico)
                try
                {
                    var icon = new System.Drawing.Icon(fileName, 256, 256);
                    var iconImage = ExtractVistaIcon(icon);
                    iconImage ??= icon.ToBitmap();

                    Sprite iconSprite;

                    using (var iconStream = new MemoryStream())
                    {
                        iconImage.Save(iconStream, System.Drawing.Imaging.ImageFormat.Png);
                        var iconTexture = new Texture(iconStream);
                        Textures.Add(iconTexture);
                        TextureFileNames.Add(fileName);

                        iconSprite = new Sprite(new Texture(iconTexture));
                    }

                    return iconSprite;
                }
                catch (Exception) { }
            }

            return null;
        }

        // http://stackoverflow.com/questions/220465/using-256-x-256-vista-icon-in-application/1945764#1945764
        // Based on: http://www.codeproject.com/KB/cs/IconExtractor.aspx
        // And a hint from: http://www.codeproject.com/KB/cs/IconLib.aspx
        public static System.Drawing.Bitmap ExtractVistaIcon(System.Drawing.Icon icoIcon)
        {
            System.Drawing.Bitmap bmpPngExtracted = null;
            try
            {
                byte[] srcBuf = null;
                using (var stream = new MemoryStream())
                {
                    icoIcon.Save(stream);
                    srcBuf = stream.ToArray();
                }
                const int SizeICONDIR = 6;
                const int SizeICONDIRENTRY = 16;
                int iCount = BitConverter.ToInt16(srcBuf, 4);
                for (int iIndex = 0; iIndex < iCount; iIndex++)
                {
                    int iWidth = srcBuf[SizeICONDIR + SizeICONDIRENTRY * iIndex];
                    int iHeight = srcBuf[SizeICONDIR + SizeICONDIRENTRY * iIndex + 1];
                    int iBitCount = BitConverter.ToInt16(
                        srcBuf,
                        SizeICONDIR + SizeICONDIRENTRY * iIndex + 6
                    );
                    if (iWidth == 0 && iHeight == 0 && iBitCount == 32)
                    {
                        int iImageSize = BitConverter.ToInt32(
                            srcBuf,
                            SizeICONDIR + SizeICONDIRENTRY * iIndex + 8
                        );
                        int iImageOffset = BitConverter.ToInt32(
                            srcBuf,
                            SizeICONDIR + SizeICONDIRENTRY * iIndex + 12
                        );
                        var destStream = new MemoryStream();
                        var writer = new BinaryWriter(destStream);
                        writer.Write(srcBuf, iImageOffset, iImageSize);
                        _ = destStream.Seek(0, SeekOrigin.Begin);
                        bmpPngExtracted = new System.Drawing.Bitmap(destStream); // This is PNG! :)
                        break;
                    }
                }
            }
            catch
            {
                return null;
            }
            return bmpPngExtracted;
        }

        public static Sprite GetSpriteFromSVG(string fileName)
        {
            int index = TextureFileNames.IndexOf(fileName);

            if (index >= 0)
            {
                // Texture Already Exists
                // move it to the end of the array and return it
                Texture texture = Textures[index];
                string name = TextureFileNames[index];

                Textures.RemoveAt(index);
                TextureFileNames.RemoveAt(index);
                Textures.Add(texture);
                TextureFileNames.Add(name);

                return new Sprite(Textures[Textures.Count - 1]);
            }
            else
            {
                // New Texture (from .svg)
                try
                {
                    // FIXME: Test svg support
                    var svg = new SKSvg();
                    svg.Load(fileName);
                    if (svg.Picture == null)
                        return null;

                    // Decide output size (you can use Picture.CullRect for intrinsic bounds)
                    var bounds = svg.Picture.CullRect;
                    int width = (int)bounds.Width;
                    int height = (int)bounds.Height;

                    // Render to a bitmap
                    using var bitmap = new SKBitmap(width, height);
                    using (var canvas = new SKCanvas(bitmap))
                    {
                        canvas.Clear(SKColors.Transparent);
                        canvas.DrawPicture(svg.Picture);
                    }

                    // Encode bitmap into PNG memory stream
                    using var image = SKImage.FromBitmap(bitmap);
                    using var data = image.Encode(SKEncodedImageFormat.Png, 100);

                    using var stream = new MemoryStream();
                    data.SaveTo(stream);
                    stream.Position = 0;

                    var texture = new Texture(stream);
                    Textures.Add(texture);
                    TextureFileNames.Add(fileName);

                    return new Sprite(new Texture(texture));
                }
                catch (Exception) { }
            }

            return null;
        }

        public static Sprite GetSpriteFromWebP(string fileName)
        {
            int index = TextureFileNames.IndexOf(fileName);

            if (index >= 0)
            {
                // Texture Already Exists
                // move it to the end of the array and return it
                Texture texture = Textures[index];
                string name = TextureFileNames[index];

                Textures.RemoveAt(index);
                TextureFileNames.RemoveAt(index);
                Textures.Add(texture);
                TextureFileNames.Add(name);

                return new Sprite(Textures[Textures.Count - 1]);
            }
            else
            {
                // New Texture (from .webp)
                //  try
                //  {
                //      byte[] fileBytes = File.ReadAllBytes(fileName);
                //      var bitmap = new Imazen.WebP.SimpleDecoder().DecodeFromBytes(
                //          fileBytes,
                //          fileBytes.Length
                //      );
                //      using (var stream = new MemoryStream())
                //      {
                //          bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                //          var texture = new Texture(stream);
                //          Textures.Add(texture);
                //          TextureFileNames.Add(fileName);

                //          return new Sprite(new Texture(texture));
                //      }
                //  }
                //  catch (Exception) { }
            }

            return null;
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
                    AnimatedImageData data = AnimatedImageDatas[index];
                    string name = AnimatedImageDataFileNames[index];

                    AnimatedImageDatas.RemoveAt(index);
                    AnimatedImageDataFileNames.RemoveAt(index);
                    AnimatedImageDatas.Add(data);
                    AnimatedImageDataFileNames.Add(name);

                    return AnimatedImageDatas[AnimatedImageDatas.Count - 1];
                }
                else
                {
                    // New AnimatedImageData
                    var image = System.Drawing.Image.FromFile(fileName);
                    var data = new AnimatedImageData();

                    // Store AnimatedImageData
                    AnimatedImageDatas.Add(data);
                    AnimatedImageDataFileNames.Add(fileName);

                    // Limit amount of Animations in Memory
                    if (AnimatedImageDatas.Count > MAX_ANIMATIONS)
                        RemoveAnimatedImage();

                    // Get Frames
                    var loadingAnimatedImage = new LoadingAnimatedImage(image, data);
                    var loadFramesThread = new Thread(
                        new ThreadStart(loadingAnimatedImage.LoadFrames)
                    )
                    {
                        Name = "AnimationLoadThread - " + fileName,
                        IsBackground = true,
                    };
                    loadFramesThread.Start();

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
                if (s == 1 && (image as AnimatedImage).Data == AnimatedImageDatas[a])
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
                        if (TextureFileNames[i].IndexOf(file) == 0)
                            s++;
                        else if (s > 0)
                            break;
                    }
                    while (TextureFileNames.Count > s)
                    {
                        if (TextureFileNames[a].IndexOf(file) == 0)
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
                string name = TextureFileNames[t].Substring(0, TextureFileNames[t].Length - 7);

                int i;
                for (i = t + 1; i < TextureFileNames.Count; i++)
                {
                    if (TextureFileNames[i].IndexOf(name) != 0)
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

    internal class LoadingAnimatedImage
    {
        private readonly System.Drawing.Image Image;
        private ImageManipulation.OctreeQuantizer Quantizer;
        private readonly AnimatedImageData Data;

        public LoadingAnimatedImage(System.Drawing.Image image, AnimatedImageData data)
        {
            Image = image;
            Data = data;
        }

        public void LoadFrames()
        {
            // Get Frame Count
            var frameDimension = new System.Drawing.Imaging.FrameDimension(
                Image.FrameDimensionsList[0]
            );
            Data.FrameCount = Image.GetFrameCount(frameDimension);
            Data.Frames = new Texture[Data.FrameCount];
            Data.FrameDelays = new int[Data.FrameCount];

            // Get Frame Delays
            byte[] frameDelays = null;
            try
            {
                var frameDelaysItem = Image.GetPropertyItem(0x5100);
                frameDelays = frameDelaysItem.Value;
                if (
                    frameDelays.Length == 0
                    || (frameDelays[0] == 0 && frameDelays.All(d => d == 0))
                )
                    frameDelays = null;
            }
            catch { }
            int defaultFrameDelay = AnimatedImage.DEFAULT_FRAME_DELAY;
            if (frameDelays != null && frameDelays.Length > 1)
                defaultFrameDelay = (frameDelays[0] + frameDelays[1] * 256) * 10;

            for (int i = 0; i < Data.FrameCount; i++)
            {
                if (Data.CancelLoading)
                    return;

                _ = Image.SelectActiveFrame(frameDimension, i);
                Quantizer = new ImageManipulation.OctreeQuantizer(255, 8);

                var quantized = Quantizer.Quantize(Image);
                var stream = new MemoryStream();
                quantized.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                Data.Frames[i] = new Texture(stream);

                stream.Dispose();

                if (Data.CancelLoading)
                    return;

                Data.Frames[i].Smooth = Data.Smooth;
                if (Data.Mipmap)
                    Data.Frames[i].GenerateMipmap();

                int fd = i * 4;
                Data.FrameDelays[i] =
                    frameDelays != null && frameDelays.Length > fd
                        ? (frameDelays[fd] + frameDelays[fd + 1] * 256) * 10
                        : defaultFrameDelay;
            }
            Data.FullyLoaded = true;
        }
    }
}
