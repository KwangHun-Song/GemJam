using System;

namespace GemMatch {
    public enum EntityIndex {
        None = 0,
        NormalPiece,
        SpawnerPiece,
        GoalPiece,
        VisibleCover,
        InvisibleCover,
    }

    public class Entity : IComparable<Entity>, ICloneable<Entity>, IEquatable<Entity> {
        public EntityModel Model { get; }
        public EntityIndex Index => Model.index;
        public Layer Layer => Model.layer;

        public virtual int Durability {
            get => Model.durability;
            set => Model.durability = value;
        }

        public virtual ColorIndex Color {
            get => Model.color;
            set => Model.color = value;
        }

        public virtual bool PreventTouch() => false;
        public virtual bool CanPassThrough() => Layer != Layer.Piece;
        public virtual bool CanAddMemory() => Index == EntityIndex.NormalPiece || Index == EntityIndex.GoalPiece;
        public virtual bool CanBeHit() => false;
        public virtual bool CanBeSplashHit() => false;

        public Entity(EntityModel model) {
            Model = model;
        }

        public virtual Entity Clone() => new Entity(Model.Clone());

        public int CompareTo(Entity other) => Layer.CompareTo(other.Layer);

        public virtual HitResultInfo Hit() {
            return new HitResultInfo(HitResult.None);
        }

        public virtual void OnUpdateEntity() { }

        public bool Equals(Entity other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Model.Equals(other.Model);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Entity)obj);
        }

        public override int GetHashCode() {
            return (Model != null ? Model.GetHashCode() : 0);
        }
    }
}