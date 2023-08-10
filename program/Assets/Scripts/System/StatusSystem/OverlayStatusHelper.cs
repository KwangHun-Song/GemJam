namespace OverlayStatusSystem {
    public static class OverlayStatusHelper {
        private static readonly OverlayStatusManager manager = new OverlayStatusManager();
        public static void Init(IOverlayStatus statusEvent) {
            manager.Init(statusEvent);
        }

        public static void Input(IOverlayStatusEvent keyObject, OverlayStatusParam inputParam) {
            manager.Input(keyObject, inputParam);
        }

        public static void Save(IOverlayStatusEvent keyObject) {
            manager.Save(keyObject);
        }
    }
}