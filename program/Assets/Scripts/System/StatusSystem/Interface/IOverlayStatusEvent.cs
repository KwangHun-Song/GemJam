using System;
using UnityEngine;

namespace OverlayStatusSystem {
    /// <summary>
    /// View가 상속할 클래스 OverlayStatus에게 콜백을 전달한다
    /// </summary>
    public interface IOverlayStatusEvent {
        GameObject gameObject { get; }
        Type GetKeyType();
    }
}