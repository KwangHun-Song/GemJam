using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;

namespace OverlayStatusSystem {
    internal class OverlayStatusManager {
#region Singleton
        private static OverlayStatusManager instance = null;
        internal static OverlayStatusManager Instance {
            get => instance ?? new OverlayStatusManager();
            set => instance = value;
        }

        private OverlayStatusManager() {
            if (Instance == null) {
                Instance = new OverlayStatusManager();
            }
        }
#endregion

        // 코인, 미션, 레벨 스테이터스바
        // Input(data를 받아온다) 따로 Save(받은 data를 저장한다)따로
        // OnSave를 구독하는 view들은 Save(외부에서 불러줌)시 스터이터스를 매개변수로 받는다
        // 모노를 상속한 view를 각 페이지가 가지고 있음
        // view는 IOverlayStatusEvent상
        // 외부에선 helper로 통신
        // listener를 iter로 돌때 오브젝트가 살아있나 확인 gameObject 또는 onasyncdestory 같은걸 걸어도 좋을듯?

        private readonly Dictionary<Type, IOverlayStatus> _statusDict = new Dictionary<Type, IOverlayStatus>();
        private readonly List<IOverlayStatus> _statusList = new List<IOverlayStatus>();

        public void Init(IOverlayStatus status) {
            this._statusList.Add(status);
            this._statusDict.Add(status.GetType(), status);
            WaitForPopListener(status.EventListener);
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