using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;

namespace OverlayStatusSystem {
    internal class OverlayStatusManager {

        private readonly Dictionary<Type, IOverlayStatus> _statusDict = new Dictionary<Type, IOverlayStatus>();
        private readonly List<IOverlayStatus> _statusList = new List<IOverlayStatus>();

        public void Init(IOverlayStatus status) {
            this._statusList.Add(status);
            this._statusDict.Add(status.GetType(), status);
            WaitForPopListener(status.EventListener).Forget();
        }

        private async UniTask WaitForPopListener(IOverlayStatusEvent status) {
            await status.gameObject.OnDestroyAsync();
            var destroyedStatus = _statusDict[status.GetType()];
            _statusDict.Remove(status.GetType());
            _statusList.Remove(destroyedStatus);
        }

        public void Input(IOverlayStatusEvent key, OverlayStatusParam inputParam) {
            _statusDict[key.GetType()].Enqueue(inputParam);
        }

        public void Save(IOverlayStatusEvent key) {
            if (_statusDict.ContainsKey(key.GetType()) == false) {
                return;
            }

            _statusDict[key.GetType()].Save();
        }
    }
}