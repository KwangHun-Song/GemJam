namespace OverlayStatusSystem {
    public static class OverlayStatusHelper {
        public static void Init(IOverlayStatus statusEvent) {
            OverlayStatusManager.Instance.Init(statusEvent);
        }

        public static void Input(IOverlayStatusEvent keyObject, OverlayStatusParam inputParam) {
            OverlayStatusManager.Instance.Input(keyObject, inputParam);
        }

        public static void Save(IOverlayStatusEvent keyObject) {
            OverlayStatusManager.Instance.Save(keyObject);
        }
    }
}