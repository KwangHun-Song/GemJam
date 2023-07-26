using System;

namespace GemMatch {
    public enum EntityIndex {
        None,
        Normal,
    }

    public enum Layer {
        None,
        Piece,
    }
    
    public abstract class Entity : IComparable<Entity> {
        public abstract EntityIndex Index { get; }
        public abstract Layer Layer { get; }
        
        public virtual int Durability { get; protected set; }
        public virtual ColorIndex Color { get; protected set; } = ColorIndex.None;
        
        public virtual bool PreventTouch => false;
        public virtual bool CanPassThrough => Layer != Layer.Piece;
        public virtual bool CanAddMemory => Index == EntityIndex.Normal;
        public virtual bool CanSplashHit => false;
        public virtual bool PreventSplashHit => false;

        public abstract Entity Clone();

        public int CompareTo(Entity other) => Layer.CompareTo(other.Layer);

        public virtual void SplashHit() { }

        public virtual void Destroy() { }
    }
}