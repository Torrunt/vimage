using System;
using SFML.Graphics;

namespace vimage
{
    internal class AnimatedImageData
    {
        public Texture[] Frames = [];
        public int[] FrameDelays = [];
        public int FrameCount = 0;
        public bool FullyLoaded = false;
        public bool CancelLoading = false;

        private bool _Smooth = true;
        public bool Smooth
        {
            get { return _Smooth; }
            set
            {
                _Smooth = value;
                if (FullyLoaded)
                {
                    foreach (var texture in Frames)
                        texture.Smooth = _Smooth;
                }
            }
        }

        private bool _Mipmap = true;
        public bool Mipmap
        {
            get { return _Mipmap; }
            set
            {
                _Mipmap = value;
                if (FullyLoaded && _Mipmap)
                {
                    foreach (var texture in Frames)
                        texture.GenerateMipmap();
                }
            }
        }

        public AnimatedImageData() { }
    }

    internal class AnimatedImage : DisplayObject
    {
        public AnimatedImageData Data;
        public Sprite Sprite;
        public new Texture Texture
        {
            get { return Sprite.Texture; }
            private set { }
        }

        public int CurrentFrame;
        public int TotalFrames
        {
            get { return Data.Frames.Length; }
            private set { }
        }

        public bool Playing = true;
        private bool _Looping = true;
        public bool Looping
        {
            get { return _Looping; }
            set
            {
                _Looping = value;
                Finished = false;
            }
        }
        public bool Finished = false;

        /// <summary> Keeps track of when to change frame. Resets on frame change. </summary>
        public float CurrentTime;
        private float CurrentFrameDelay;

        /// <summary>Default Frame Delay for animated images that don't define it.</summary>
        public static readonly int DEFAULT_FRAME_DELAY = 100;

        public AnimatedImage(AnimatedImageData data)
        {
            Data = data;

            Sprite = new Sprite(data.Frames[0]);
            AddChild(Sprite);

            CurrentTime = 0;
            CurrentFrameDelay = data.FrameDelays[0];
        }

        public bool Update(float dt)
        {
            if (!Playing)
                return false;

            CurrentTime += dt;

            while (CurrentTime > CurrentFrameDelay)
            {
                if (Looping || CurrentFrame < TotalFrames - 1)
                {
                    if (CurrentFrame == TotalFrames - 1)
                        _ = SetFrame(0);
                    else
                        NextFrame();
                }
                else
                    Finished = true;

                if (CurrentFrameDelay == 0)
                    CurrentTime = 0;
                else
                    CurrentTime -= CurrentFrameDelay;

                return true;
            }

            return false;
        }

        public bool SetFrame(int number)
        {
            if (number >= TotalFrames)
                return false;

            if (!Data.FullyLoaded && Data.Frames[number] == null)
                return false; // Hang if next frame hasn't loaded yet

            CurrentFrame = number;
            Finished = CurrentFrame == TotalFrames - 1;

            Sprite.Texture = Data.Frames[CurrentFrame];
            CurrentFrameDelay = Data.FrameDelays[CurrentFrame];

            return true;
        }

        public void NextFrame()
        {
            _ = SetFrame(Math.Min(CurrentFrame + 1, TotalFrames));
        }

        public void PrevFrame()
        {
            _ = SetFrame(Math.Max(CurrentFrame - 1, 0));
        }

        public void Stop()
        {
            Playing = false;
        }

        public void Play()
        {
            Playing = true;
        }

        public void GotoAndPlay(int number)
        {
            _ = SetFrame(number);
            Play();
        }

        public void GotoAndStop(int number)
        {
            _ = SetFrame(number);
            Stop();
        }
    }
}
