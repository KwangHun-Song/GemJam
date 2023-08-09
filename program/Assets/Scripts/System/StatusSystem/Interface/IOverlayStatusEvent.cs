using System;
using UnityEngine;

namespace OverlayStatusSystem {
    public interface IOverlayStatusEvent {
        GameObject gameObject { get; }
        Type GetKeyType();
    }
}