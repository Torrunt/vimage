using DevIL.Unmanaged;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Tao.OpenGl;

namespace vimage
{
    /// <summary>
    /// Graphics Manager.
    /// Loads and stores Textures and AnimatedImageDatas.
    /// </summary>
    internal class Graphics
    {
        private static readonly List<Texture> Textures = new List<Texture>();
        private static readonly List<string> TextureFileNames = new List<string>();

        private static readonly List<AnimatedImageData> AnimatedImageDatas = new List<AnimatedImageData>();
        private static readonly List<string> AnimatedImageDataFileNames = new List<string>();

        private static readonly List<DisplayObject> SplitTextures = new List<DisplayObject>();
        private static readonly List<string> SplitTextureFileNames = new List<string>();

        public static uint MAX_TEXTURES = 80;
        public static uint MAX_ANIMATIONS = 8;
        public static int TextureMaxSize = (int)Texture.MaximumSize;

        public static bool UseDevil = false;

        public static void InitDevIL()
        {
            try
            {
                IL.Initialize();
                UseDevil = true;
            }
            catch (DllNotFoundException)
            {
                System.Windows.Forms.MessageBox.Show("vimage failed to find DevIL.dll.\nIf problem persists, try disabling DevIL in the settings.", "vimage - DevIL.dll not found");
            }
        }

        public static dynamic GetTexture(string fileName)
        {
            int index = TextureFileNames.IndexOf(fileName);
            int splitTextureIndex = SplitTextureFileNames.Count == 0 ? -1 : SplitTextureFileNames.IndexOf(fileName);

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
                        int imageID = IL.GenerateImage();
                        IL.BindImage(imageID);

                        _ = IL.Enable(ILEnable.AbsoluteOrigin);
                        IL.SetOriginLocation(DevIL.OriginLocation.UpperLeft);

                        bool loaded = IL.LoadImageFromStream(fileStream);

                        if (loaded)
                        {
                            if (IL.GetImageInfo().Width > TextureMaxSize || IL.GetImageInfo().Height > TextureMaxSize)
                            {
                                // Large Image split-up into multiple textures
                                // (image is larger than GPU's max texture size)
                                textureLarge = GetLargeTextureFromBoundImage(TextureMaxSize / 2, fileName);
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
                        IL.DeleteImages(new ImageID[] { imageID });
                    }
                    else
                    {
                        // Load image via SFML
                        try
                        {
                            using (Image image = new Image(fileStream))
                            {
                                Vector2u imageSize = image.Size;

                                if (imageSize.X > TextureMaxSize || imageSize.Y > TextureMaxSize)
                                {
                                    // Large Image split-up into multiple textures
                                    textureLarge = GetLargeTextureFromSFMLImage(TextureMaxSize, image, fileName);
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
                            System.Windows.Forms.MessageBox.Show("Failed to load image:\n" + fileName + ".\n\nTry changing \"Use DevIL\" on in the settings to help with this issue.", "vimage - SFML Image Loading Failed");
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
            bool success = IL.ConvertImage(DevIL.DataFormat.RGBA, DevIL.DataType.UnsignedByte);

            if (!success)
                return null;

            int width = IL.GetImageInfo().Width;
            int height = IL.GetImageInfo().Height;

            Texture texture = new Texture((uint)width, (uint)height);
            Texture.Bind(texture);
            {
                Gl.glTexImage2D(
                    Gl.GL_TEXTURE_2D, 0, IL.GetInteger(ILIntegerMode.ImageBytesPerPixel),
                    width, height, 0,
                    IL.GetInteger(ILIntegerMode.ImageFormat), ILDefines.IL_UNSIGNED_BYTE,
                    IL.GetData()
                    );
            }
            Texture.Bind(null);

            return texture;
        }
        private static DisplayObject GetLargeTextureFromBoundImage(int sectionSize, string fileName = "")
        {
            bool success = IL.ConvertImage(DevIL.DataFormat.RGBA, DevIL.DataType.UnsignedByte);

            if (!success)
                return null;

            DisplayObject largeTexture = new DisplayObject();

            Vector2i size = new Vector2i(IL.GetImageInfo().Width, IL.GetImageInfo().Height);
            Vector2u amount = new Vector2u((uint)Math.Ceiling(size.X / (float)sectionSize), (uint)Math.Ceiling(size.Y / (float)sectionSize));
            Vector2i currentSize = new Vector2i(size.X, size.Y);
            Vector2i pos = new Vector2i();

            for (int iy = 0; iy < amount.Y; iy++)
            {
                int h = Math.Min(currentSize.Y, sectionSize);
                currentSize.Y -= h;
                currentSize.X = size.X;

                for (int ix = 0; ix < amount.X; ix++)
                {
                    int w = Math.Min(currentSize.X, sectionSize);
                    currentSize.X -= w;

                    Texture texture = new Texture((uint)w, (uint)h);
                    IntPtr partPtr = Marshal.AllocHGlobal(w * h * 4);
                    _ = IL.CopyPixels(pos.X, pos.Y, 0, w, h, 1, DevIL.DataFormat.RGBA, DevIL.DataType.UnsignedByte, partPtr);
                    Texture.Bind(texture);
                    {
                        Gl.glTexImage2D(
                            Gl.GL_TEXTURE_2D, 0, IL.GetInteger(ILIntegerMode.ImageBytesPerPixel),
                            w, h, 0,
                            IL.GetInteger(ILIntegerMode.ImageFormat), ILDefines.IL_UNSIGNED_BYTE,
                            partPtr);
                    }
                    Texture.Bind(null);
                    Marshal.FreeHGlobal(partPtr);

                    Sprite sprite = new Sprite(texture)
                    {
                        Position = new Vector2f(pos.X, pos.Y)
                    };
                    largeTexture.AddChild(sprite);

                    if (fileName != "")
                    {
                        Textures.Add(texture);
                        TextureFileNames.Add(fileName + "_" + ix.ToString("00") + "_" + iy.ToString("00") + "^");
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

        private static Texture GetTextureFromSFMLImage(Image image, IntRect area = default)
        {
            Vector2u imageSize = image.Size;
            byte[] bytes = image.Pixels;

            if (area == default)
                area = new IntRect(0, 0, (int)imageSize.X, (int)imageSize.Y);

            Texture texture = new Texture((uint)area.Width, (uint)area.Height);

            byte[] pixels;
            const int blockSize = 4;
            if (area.Width < imageSize.X || area.Height < imageSize.Y)
            {
                var crop = new byte[area.Width * area.Height * blockSize];

                for (var line = 0; line <= area.Height - 1; line++)
                {
                    var sourceIndex = ((area.Top + line) * imageSize.X + area.Left) * blockSize;
                    var destinationIndex = line * area.Width * blockSize;

                    Array.Copy(bytes, sourceIndex, crop, destinationIndex, area.Width * blockSize);
                }

                pixels = crop;
            }
            else
                pixels = bytes;

            Texture.Bind(texture);
            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA, area.Width, area.Height, 0, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, pixels);
            Texture.Bind(null);

            return texture;
        }
        private static DisplayObject GetLargeTextureFromSFMLImage(int sectionSize, Image image, string fileName = "")
        {
            Vector2u imageSize = image.Size;
            Vector2u amount = new Vector2u((uint)Math.Ceiling((float)imageSize.X / sectionSize), (uint)Math.Ceiling((float)imageSize.Y / sectionSize));
            Vector2i currentSize = new Vector2i((int)imageSize.X, (int)imageSize.Y);
            Vector2i pos = new Vector2i();


            DisplayObject largeTexture = new DisplayObject();

            for (int iy = 0; iy < amount.Y; iy++)
            {
                int h = Math.Min(currentSize.Y, sectionSize);
                currentSize.Y -= h;
                currentSize.X = (int)imageSize.X;

                for (int ix = 0; ix < amount.X; ix++)
                {
                    int w = Math.Min(currentSize.X, sectionSize);
                    currentSize.X -= w;

                    Texture texture = GetTextureFromSFMLImage(image, new IntRect(pos.X, pos.Y, w, h));
                    Sprite sprite = new Sprite(texture)
                    {
                        Position = new Vector2f(pos.X, pos.Y)
                    };
                    largeTexture.AddChild(sprite);

                    if (fileName != "")
                    {
                        Textures.Add(texture);
                        TextureFileNames.Add(fileName + "_" + ix.ToString("00") + "_" + iy.ToString("00") + "^");
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
                // New Texture (from .ico)
                try
                {
                    System.Drawing.Icon icon = new System.Drawing.Icon(fileName, 256, 256);
                    System.Drawing.Bitmap iconImage = ExtractVistaIcon(icon);
                    if (iconImage == null)
                        iconImage = icon.ToBitmap();

                    Sprite iconSprite;

                    using (MemoryStream iconStream = new MemoryStream())
                    {
                        iconImage.Save(iconStream, System.Drawing.Imaging.ImageFormat.Png);
                        Texture iconTexture = new Texture(iconStream);
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
                using (MemoryStream stream = new MemoryStream())
                { icoIcon.Save(stream); srcBuf = stream.ToArray(); }
                const int SizeICONDIR = 6;
                const int SizeICONDIRENTRY = 16;
                int iCount = BitConverter.ToInt16(srcBuf, 4);
                for (int iIndex = 0; iIndex < iCount; iIndex++)
                {
                    int iWidth = srcBuf[SizeICONDIR + SizeICONDIRENTRY * iIndex];
                    int iHeight = srcBuf[SizeICONDIR + SizeICONDIRENTRY * iIndex + 1];
                    int iBitCount = BitConverter.ToInt16(srcBuf, SizeICONDIR + SizeICONDIRENTRY * iIndex + 6);
                    if (iWidth == 0 && iHeight == 0 && iBitCount == 32)
                    {
                        int iImageSize = BitConverter.ToInt32(srcBuf, SizeICONDIR + SizeICONDIRENTRY * iIndex + 8);
                        int iImageOffset = BitConverter.ToInt32(srcBuf, SizeICONDIR + SizeICONDIRENTRY * iIndex + 12);
                        MemoryStream destStream = new MemoryStream();
                        BinaryWriter writer = new BinaryWriter(destStream);
                        writer.Write(srcBuf, iImageOffset, iImageSize);
                        _ = destStream.Seek(0, SeekOrigin.Begin);
                        bmpPngExtracted = new System.Drawing.Bitmap(destStream); // This is PNG! :)
                        break;
                    }
                }
            }
            catch { return null; }
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
                    Svg.SvgDocument svg = Svg.SvgDocument.Open(fileName);
                    System.Drawing.Bitmap bitmap = svg.Draw();
                    using (MemoryStream stream = new MemoryStream())
                    {
                        bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                        Texture texture = new Texture(stream);
                        Textures.Add(texture);
                        TextureFileNames.Add(fileName);

                        return new Sprite(new Texture(texture));
                    }
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
                try
                {
                    byte[] fileBytes = File.ReadAllBytes(fileName);
                    System.Drawing.Bitmap bitmap = new Imazen.WebP.SimpleDecoder().DecodeFromBytes(fileBytes, fileBytes.Length);
                    using (MemoryStream stream = new MemoryStream())
                    {
                        bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                        Texture texture = new Texture(stream);
                        Textures.Add(texture);
                        TextureFileNames.Add(fileName);

                        return new Sprite(new Texture(texture));
                    }
                }
                catch (Exception) { }
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
                    System.Drawing.Image image = System.Drawing.Image.FromFile(fileName);
                    AnimatedImageData data = new AnimatedImageData();

                    // Store AnimatedImageData
                    AnimatedImageDatas.Add(data);
                    AnimatedImageDataFileNames.Add(fileName);

                    // Limit amount of Animations in Memory
                    if (AnimatedImageDatas.Count > MAX_ANIMATIONS)
                        RemoveAnimatedImage();

                    // Get Frames
                    LoadingAnimatedImage loadingAnimatedImage = new LoadingAnimatedImage(image, data);
                    Thread loadFramesThread = new Thread(new ThreadStart(loadingAnimatedImage.LoadFrames))
                    {
                        Name = "AnimationLoadThread - " + fileName,
                        IsBackground = true
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

                int splitIndex = SplitTextureFileNames.Count == 0 ? -1 : SplitTextureFileNames.IndexOf(name);
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
            System.Drawing.Imaging.FrameDimension frameDimension = new System.Drawing.Imaging.FrameDimension(Image.FrameDimensionsList[0]);
            Data.FrameCount = Image.GetFrameCount(frameDimension);
            Data.Frames = new Texture[Data.FrameCount];
            Data.FrameDelays = new int[Data.FrameCount];

            // Get Frame Delays
            byte[] frameDelays = null;
            try
            {
                System.Drawing.Imaging.PropertyItem frameDelaysItem = Image.GetPropertyItem(0x5100);
                frameDelays = frameDelaysItem.Value;
                if (frameDelays.Length == 0 || (frameDelays[0] == 0 && frameDelays.All(d => d == 0))) frameDelays = null;
            }
            catch { }
            int defaultFrameDelay = AnimatedImage.DEFAULT_FRAME_DELAY;
            if (frameDelays != null && frameDelays.Length > 1) defaultFrameDelay = (frameDelays[0] + frameDelays[1] * 256) * 10;

            for (int i = 0; i < Data.FrameCount; i++)
            {
                if (Data.CancelLoading)
                    return;

                _ = Image.SelectActiveFrame(frameDimension, i);
                Quantizer = new ImageManipulation.OctreeQuantizer(255, 8);

                System.Drawing.Bitmap quantized = Quantizer.Quantize(Image);
                MemoryStream stream = new MemoryStream();
                quantized.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                Data.Frames[i] = new Texture(stream);

                stream.Dispose();

                if (Data.CancelLoading)
                    return;

                Data.Frames[i].Smooth = Data.Smooth;
                if (Data.Mipmap) Data.Frames[i].GenerateMipmap();

                int fd = i * 4;
                Data.FrameDelays[i] = frameDelays != null && frameDelays.Length > fd ? (frameDelays[fd] + frameDelays[fd + 1] * 256) * 10 : defaultFrameDelay;
            }
            Data.FullyLoaded = true;
        }
    }

}
