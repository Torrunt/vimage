using SFML.Graphics;
using System.Collections.Generic;
using System;
using Tao.DevIl;
using Tao.OpenGl;
using System.IO;
using System.Runtime.InteropServices;

namespace vimage
{
    /// <summary>
    /// Graphics Manager.
    /// Loads and stores Textures and AnimatedImageDatas.
    /// </summary>
    class Graphics
    {
        private static List<Texture> Textures = new List<Texture>();
        private static List<string> TextureFileNames = new List<string>();

        private static List<AnimatedImageData> AnimatedImageDatas = new List<AnimatedImageData>();
        private static List<string> AnimatedImageDataFileNames = new List<string>();

        public static Sprite GetSprite(string fileName, bool smooth = false)
        {
            Sprite sprite = new Sprite(GetTexture(fileName));
            sprite.Texture.Smooth = smooth;

            return sprite;
        }
        /// <param name="imageNum">The Active Image Number (for animated gifs).</param>
        public static Texture GetTexture(string fileName)
        {
            int index = TextureFileNames.IndexOf(fileName);

            if (index >= 0)
            {
                // Texture Already Exists
                return Textures[index];
            }
            else
            {
                // New Texture
                int imageID = 0;

                Il.ilGenImages(1, out imageID);
                Il.ilBindImage(imageID);

                if (Il.ilLoadImage(fileName))
                {
                    Texture texture = GetTextureFromBoundImage();

                    Textures.Add(texture);
                    TextureFileNames.Add(fileName);

                    return texture;
                }
                Il.ilDeleteImage(imageID);

                return null;
            }
        }
        private static Texture GetTextureFromBoundImage(int imageNum = 0)
        {
            Il.ilActiveImage(imageNum);

            bool success = Il.ilConvertImage(Il.IL_RGBA, Il.IL_UNSIGNED_BYTE);

            if (!success)
                return null;

            int width = Il.ilGetInteger(Il.IL_IMAGE_WIDTH);
            int height = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT);

            int image = 0;
            Gl.glGenTextures(1, out image);

            Texture texture = new Texture((uint)width, (uint)height);
            Texture.Bind(texture);
            {
                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);
                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);
                Gl.glTexImage2D(
                    Gl.GL_TEXTURE_2D, 0,
                    Il.ilGetInteger(Il.IL_IMAGE_BPP),
                    Il.ilGetInteger(Il.IL_IMAGE_WIDTH),
                    Il.ilGetInteger(Il.IL_IMAGE_HEIGHT), 0,
                    Il.ilGetInteger(Il.IL_IMAGE_FORMAT), Gl.GL_UNSIGNED_BYTE,
                    Il.ilGetData()
                    );

                Gl.glBegin(Gl.GL_QUADS);
                {
                    Gl.glTexCoord2f(0.0f, 0.0f); Gl.glVertex2f(-width / 2.0f, height / 2.0f);
                    Gl.glTexCoord2f(0.0f, 1.0f); Gl.glVertex2f(-width / 2.0f, height / 2.0f);
                    Gl.glTexCoord2f(1, 1); Gl.glVertex2f(1, -1);
                    Gl.glTexCoord2f(1, 0); Gl.glVertex2f(1, 1);
                }
                Gl.glEnd();
            }
            Texture.Bind(null);

            Gl.glDeleteTextures(1, ref image);

            return texture;
        }

        public static int NumberOfFramesInImage(string fileName)
        {
            int imageid = 0;
            Il.ilGenImages(1, out imageid);
            Il.ilBindImage(imageid);

            bool success = Il.ilLoadImage(fileName);
            if (!success)
                return 0;

            int no = Il.ilGetInteger(Il.IL_NUM_IMAGES) + 1;

            Il.ilDeleteImage(imageid);

            return no;
        }

        /// <param name="filename">Animated Image (ie: animated gif).</param>
        public static AnimatedImage GetAnimatedImage(string fileName)
        {
            return new AnimatedImage(GetAnimatedImageData(fileName));
        }
        /// <param name="filename">Animated Image (ie: animated gif).</param>
        public static AnimatedImageData GetAnimatedImageData(string fileName)
        {
            int index = TextureFileNames.IndexOf(fileName);

            if (index >= 0)
            {
                // AnimatedImageData Already Exists
                return AnimatedImageDatas[index];
            }
            else
            {
                // New AnimatedImageData
                System.Drawing.Image image = System.Drawing.Image.FromFile(fileName);
                AnimatedImageData data = new AnimatedImageData();
                ImageManipulation.OctreeQuantizer quantizer;

                System.Drawing.Imaging.FrameDimension frameDimension = new System.Drawing.Imaging.FrameDimension(image.FrameDimensionsList[0]);
                for (int i = 0; i < image.GetFrameCount(frameDimension); i++)
                {
                    image.SelectActiveFrame(frameDimension, i);
                    quantizer = new ImageManipulation.OctreeQuantizer(255, 8);

                    System.Drawing.Bitmap quantized = quantizer.Quantize(image);
                    MemoryStream stream = new MemoryStream();
                    quantized.Save(stream, System.Drawing.Imaging.ImageFormat.Gif);
                    data.Frames.Add(new Texture(stream));

                    stream.Dispose();

                    data.Frames[i].Smooth = true;
                }

                System.Drawing.Imaging.PropertyItem frameDelay = image.GetPropertyItem(0x5100);
                int frameDuration = (frameDelay.Value[0] + frameDelay.Value[1] * 256) * 10;
                if (frameDuration != 0)
                    data.FrameDuration = frameDuration;
                else
                    data.FrameDuration = AnimatedImage.DEFAULT_FRAME_DURATION;
                

                AnimatedImageDatas.Add(data);
                AnimatedImageDataFileNames.Add(fileName);

                return data;
            }
        }

    }
}
