using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace OverlayStatusSystem {
    public class OverlayStatusEvent<Status> : MonoBehaviour, IOverlayStatusEvent where Status : IOverlayStatus {
        public Type GetKeyType() => typeof(Status);

        public virtual async UniTask Get<T>(T amount) {
            OverlayStatusHelper.Input(this, new OverlayStatusParam(amount));
            await Animate();
            OverlayStatusHelper.Save(this);
        }

        public virtual UniTask Animate() {
            return UniTask.CompletedTask;
        }
    }
}