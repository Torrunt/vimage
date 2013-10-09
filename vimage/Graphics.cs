using SFML.Graphics;
using System.Collections.Generic;
using System;
using Tao.DevIl;
using Tao.OpenGl;
using System.IO;
using System.Runtime.InteropServices;

namespace vimage
{
    public class TDrawable : Transformable, Drawable
    {
        public void Draw(RenderTarget Target, RenderStates states) { }
    }

    class Graphics
    {
        private static List<Texture> Textures = new List<Texture>();
        private static List<string> TextureFileNames = new List<string>();

        public static Sprite GetSprite(string filename, bool smooth = false)
        {
            Sprite sprite = new Sprite(GetTexture(filename));
            sprite.Texture.Smooth = smooth;

            return sprite;
        }
        public static Texture GetTexture(string filename)
        {
            int index = TextureFileNames.IndexOf(filename);

            if (index >= 0)
            {
                // Texture Already Exists
                return Textures[index];
            }
            else
            {
                // New Texture
                int imageid = 0;
                int image = 0;
                int width = 0;
                int height = 0;

                Il.ilGenImages(1, out imageid);
                Il.ilBindImage(imageid);

                bool success = Il.ilLoadImage(filename);

                if (success)
                {
                    success = Il.ilConvertImage(Il.IL_RGBA, Il.IL_UNSIGNED_BYTE);

                    if (!success)
                        return null;

                    width = Il.ilGetInteger(Il.IL_IMAGE_WIDTH);
                    height = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT);

                    Gl.glGenTextures(1, out image);

                    Texture texture = new Texture((uint)width, (uint)height);
                    Texture.Bind(texture);
                    {
                        Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);
                        Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);
                        Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0,
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

                    Textures.Add(texture);
                    TextureFileNames.Add(filename);

                    return texture;
                }
                Il.ilDeleteImage(imageid);

                return null;
            }
        }
    }
}
