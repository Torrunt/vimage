using System;
using System.Collections.Generic;
using System.Linq;
using SFML.Graphics;

namespace vimage
{
    class AnimatedImageData
    {
        public List<Texture> Frames = new List<Texture>();
        public int FramesCount = 0;
        public int FrameDuration = 100;

        private bool _Smooth = true;
        public bool Smooth 
        {
            get { return _Smooth; }
            set
            {
                _Smooth = value;
                foreach (Texture texture in Frames)
                    texture.Smooth = _Smooth;
            }
        }

        public AnimatedImageData() { }
    }

    class AnimatedImage : DisplayObject
    {
        public AnimatedImageData Data;
        public Sprite Sprite;
        public new Texture Texture { get { return Sprite.Texture; } private set { } }
        public Color _Color = Color.White;
        public Color Color { get { return _Color; } set { _Color = value; if (Sprite != null) Sprite.Color = _Color; } }

        public int CurrentFrame;
        public int TotalFrames { get { return Data.Frames.Count; } private set { } }

        public bool Playing = true;
        private bool _Looping = true;
        public bool Looping { get { return _Looping; } set { _Looping = value; Finished = false; } }
        public bool Finished = false;

        /// <summary> Keeps track of when to change frame. Resets on frame change. </summary>
        public float CurrentTime;
        private float CurrentFrameLength;

        /// <summary>Default Frame Duration for animated image that don't define it.</summary>
        public static readonly int DEFAULT_FRAME_DURATION = 100;

        public AnimatedImage(AnimatedImageData data)
        {
            Data = data;
            
            Sprite = new Sprite(data.Frames[0]);
            AddChild(Sprite);

            CurrentTime = 0;
            CurrentFrameLength = data.FrameDuration;
        }

        public bool Update(float dt)
        {
            if (!Playing)
                return false;

            CurrentTime += dt;

            while (CurrentTime > CurrentFrameLength)
            {
                // If next frame is yet to be loaded in - don't loop or continue animation until it is
                if (TotalFrames != Data.FramesCount && Looping && CurrentFrame == TotalFrames - 1 && CurrentFrame < Data.FramesCount - 1)
                    continue;

                if (Looping || CurrentFrame < TotalFrames - 1)
                {
                    if (CurrentFrame == TotalFrames - 1)
                        SetFrame(0);
                    else
                        NextFrame();
                }
                else
                    Finished = true;
                
                if (CurrentFrameLength == 0)
                    CurrentTime = 0;
                else
                    CurrentTime -= CurrentFrameLength;

                return true;
            }

            return false;
        }

        public bool SetFrame(int number)
        {
            if (number >= TotalFrames)
                return false;

            CurrentFrame = number;
            Finished = CurrentFrame == TotalFrames - 1;
            
            RemoveChild(Sprite);
            Sprite = new Sprite(Data.Frames[CurrentFrame]);
            if (Color != Color.White)
                Sprite.Color = Color;
            AddChild(Sprite);

            CurrentFrameLength = Data.FrameDuration;
            
            return true;
        }

        public void NextFrame() { SetFrame(Math.Min(CurrentFrame + 1, TotalFrames)); }
        public void PrevFrame() { SetFrame(Math.Max(CurrentFrame - 1, 0)); }

        public void Stop() { Playing = false; }
        public void Play() { Playing = true; }

        public void GotoAndPlay(int number) { SetFrame(number); Play(); }
        public void GotoAndStop(int number) { SetFrame(number); Stop(); }

    }
}
