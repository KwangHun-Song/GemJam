using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;

namespace OverlayStatusSystem {
    internal class OverlayStatusManager {

        private readonly Dictionary<Type, List<IOverlayStatus>> _statusDict = new Dictionary<Type, List<IOverlayStatus>>();
        private readonly List<IOverlayStatus> _statusList = new List<IOverlayStatus>();

        public void Init(IOverlayStatus status) {
            Add(status);
            WaitForPopListener(status.EventListener).Forget();
        }

        private void Add(IOverlayStatus status) {
            this._statusList.Add(status);
            this._statusDict[status.GetType()].Add(status);
        }

        private async UniTask WaitForPopListener(IOverlayStatusEvent statusListener) {
            var type = statusListener.GetKeyType();
            await statusListener.gameObject.OnDestroyAsync();
            var destroyedListener = _statusDict[type]
                .Find(s => s.EventListener == statusListener);
            _statusList.Remove(destroyedListener);
        }

        public void Input(IOverlayStatusEvent key, OverlayStatusParam inputParam) {
            foreach (var listener in _statusDict[key.GetKeyType()]) {
                listener.Enqueue(inputParam);
            }
        }

        public void Save(IOverlayStatusEvent key) {
            if (_statusDict.ContainsKey(key.GetKeyType()) == false) {
                return;
            }
            foreach (var listener in _statusDict[key.GetKeyType()]) {
                listener.Save();
            }
        }
    }
}