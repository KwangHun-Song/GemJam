using System;

namespace OverlayStatusSystem {
    public static class OverlayStatusHelper {
        public static void Input(IOverlayStatusParam inputParam) {
            OverlayStatusManager.Instance.Input(inputParam);
        }
    }

    public interface IOverlayStatusParam {
        Type GetType();
        object Value { get; }
    }
}