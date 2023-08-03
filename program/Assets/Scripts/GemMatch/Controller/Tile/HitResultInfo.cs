namespace GemMatch {
    public enum HitResult { None, Hit, Destroyed }
    public struct HitResultInfo {
        public Entity entity;
        public HitResult hitResult;
    }
}