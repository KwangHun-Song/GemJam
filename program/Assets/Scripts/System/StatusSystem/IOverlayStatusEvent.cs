using UnityEngine;

namespace OverlayStatusSystem {
    internal interface IOverlayStatusEvent {
        GameObject gameObject { get; }
        internal void OnSave(IOverlayStatus status);
    }
}