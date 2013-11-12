using SFML.Graphics;
using SFML.Window;
using System.Collections.Generic;
using System;

namespace vimage
{

    class DisplayObject : Transformable, Drawable
    {
        private List<dynamic> Children = new List<dynamic>();
        private int DrawListIndex = 0;
        public DisplayObject Parent = null;

        public bool Visible = true;

        public int NumChildren { get { return Children.Count; } }
        public void AddChild(dynamic child)
        {
            Children.Add(child);
            if (child is DisplayObject)
            {
                child.Parent = this;
                child.OnAdded();
            }
        }
        public void AddChildAt(dynamic child, int index)
        {
            Children.Insert(index, child);
            if (child is DisplayObject)
            {
                child.Parent = this;
                child.OnAdded();
            }
        }
        public void RemoveChild(dynamic child)
        {
            for (int i = 0; i < Children.Count; i++)
            {
                if (Children[i].Equals(child))
                {
                    if (child is DisplayObject)
                    {
                        child.OnRemoved();
                        child.Parent = null;
                    }

                    Children.RemoveAt(i);
                    if (i <= DrawListIndex)
                        DrawListIndex--;
                    break;
                }
            }
        }
        public void RemoveChildAt(int index) { RemoveChild(GetChildAt(index)); }
        public void Clear()
        {
            while (NumChildren > 0)
                RemoveChildAt(0);

            Children.Clear();
            DrawListIndex = 0;
        }

        public dynamic GetChildAt(int i) { return Children[i]; }

        public virtual void OnAdded() { }
        public virtual void OnRemoved() { }

        public void Draw(RenderTarget Target, RenderStates states)
        {
            states.Transform *= Transform;
            for (DrawListIndex = 0; DrawListIndex < Children.Count; DrawListIndex++)
            {
                if (!(Children[DrawListIndex] is DisplayObject) || Children[DrawListIndex].Visible)
                    Children[DrawListIndex].Draw(Target, states);
            }
        }

        public float X
        {
            get { return Position.X; }
            set { Position = new Vector2f(value, Position.Y); }
        }
        public float Y
        {
            get { return Position.Y; }
            set { Position = new Vector2f(Position.X, value); }
        }
        public void SetPosition(float x, float y) { Position = new Vector2f(x, y); }
        public void SetPosition(Vector2f pos) { Position = pos; }

        public void Move(float offsetX, float offsetY) { Position = new Vector2f(X + offsetX, Y + offsetY); }
        public void Move(Vector2f offset) { Position = new Vector2f(X + offset.X, Y + offset.Y); }

        public float ScaleX
        {
            get { return Scale.X; }
            set { Scale = new Vector2f(value, Scale.Y); }
        }
        public float ScaleY
        {
            get { return Scale.Y; }
            set { Scale = new Vector2f(Scale.X, value); }
        }
        public void SetScale(float scaleX, float scaleY) { Scale = new Vector2f(scaleX, scaleY); }
        public void SetScale(float scale) { Scale = new Vector2f(scale, scale); }
        public void SetScale(Vector2f scale) { Scale = scale; }

        public void Rotate(float amount)
        {
            if (Rotation + amount > 180)
                Rotation = Rotation + amount - 360;
            else if (Rotation + amount < -180)
                Rotation = Rotation + amount + 360;
            else
                Rotation += amount;
        }

    }

    class Layer : DisplayObject { }
}
