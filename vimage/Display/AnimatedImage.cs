using System;
using SFML.Graphics;

namespace vimage.Display
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
            get => Sprite.Texture;
            private set { }
        }

        public int CurrentFrame;
        public int TotalFrames
        {
            get => Data.Frames.Length;
            private set { }
        }

        public bool Playing = true;
        public bool Looping
        {
            get;
            set
            {
                field = value;
                Finished = false;
            }
        } = true;
        public bool Finished = false;

        /// <summary> Keeps track of when to change frame. Resets on frame change. </summary>
        public float CurrentTime;

        public float SpeedMultiplier
        {
            get;
            set
            {
                if (field == value)
                    return;
                field = value;
                CurrentTime = 0;
            }
        } = 1.0f;

        /// <summary>Default Frame Delay for animated images that don't define it.</summary>
        public static readonly int DEFAULT_FRAME_DELAY = 100;

        public AnimatedImage(AnimatedImageData data)
        {
            Data = data;

            Sprite = new Sprite(data.Frames[0]);
            AddChild(Sprite);

            CurrentTime = 0;
        }

        public bool Update(float dt)
        {
            if (!Playing)
                return false;

            CurrentTime += dt * SpeedMultiplier;
            var frame = CurrentFrame;

            while (CurrentTime >= Data.FrameDelays[frame])
            {
                CurrentTime -= Data.FrameDelays[frame];

                if (frame == TotalFrames - 1)
                {
                    if (!Looping)
                    {
                        Finished = true;
                        Playing = false;
                        break;
                    }
                    frame = 0;
                }
                else
                    frame++;
            }

            if (frame == CurrentFrame)
                return false;

            return SetFrame(frame);
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
