using System;
using System.Collections.Generic;

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

        private readonly Dictionary<Type, object> _inputPool = new Dictionary<Type, object>();
        private readonly List<IOverlayStatus> _statusList = new List<IOverlayStatus>();
        private event  Action<List<IOverlayStatus>> OnSave;

        public void Input(IOverlayStatusParam inputParam) {
            _inputPool[inputParam.GetType()] = inputParam.Value;
        }

        public void Save() {
            foreach (IOverlayStatus status in _statusList) {
                if (_inputPool.TryGetValue(status.GetType(), out object input)) {
                    status.Save(input);
                    _inputPool.Remove(status.GetType());
                }
            }
            OnSave?.Invoke(_statusList);
        }
    }
}