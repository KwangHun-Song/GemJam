namespace GemMatch {
    public enum HitResult { None, Hit, Destroyed }
    public struct HitResultInfo {
        public readonly HitResult hitResult;
        public bool prevent;

        public HitResultInfo(HitResult hitResult, bool prevent = false) {
            this.hitResult = hitResult;
            this.prevent = prevent;
        }
    }
}