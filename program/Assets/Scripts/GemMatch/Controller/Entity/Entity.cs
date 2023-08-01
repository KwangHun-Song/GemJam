using System;

namespace GemMatch {
    public enum EntityIndex {
        None,
        NormalPiece,
    }

    public enum Layer {
        None,
        Piece,
    }
    
    public class Entity : IComparable<Entity> {
        public EntityModel Model { get; }
        public virtual EntityIndex Index => Model.index;
        public virtual Layer Layer => Model.layer;

        public virtual int Durability {
            get => Model.durability;
            set { }
        }

        public virtual ColorIndex Color {
            get => Model.color;
            set { }
        }
        
        public virtual bool PreventTouch() => false;
        public virtual bool CanPassThrough() => Layer != Layer.Piece;
        public virtual bool CanAddMemory() => Index == EntityIndex.NormalPiece;
        public virtual bool CanSplashHit() => false;
        public virtual bool PreventSplashHit() => false;

        public Entity(EntityModel model) { Model = model; }
        public virtual Entity Clone() => new Entity(Model);

        public int CompareTo(Entity other) => Layer.CompareTo(other.Layer);

        public virtual void SplashHit() { }

        public virtual void Destroy() { }
    }
}