namespace GemMatch {
    public enum EntityIndex {
        None,
        Normal,
    }

    public enum Layer {
        None,
        Piece,
    }
    
    public abstract class Entity {
        public abstract EntityIndex Index { get; }
        public abstract Layer Layer { get; }
        
        public virtual ColorIndex Color => ColorIndex.None;
        public virtual bool CanTouch => Index == EntityIndex.Normal;
    }
}