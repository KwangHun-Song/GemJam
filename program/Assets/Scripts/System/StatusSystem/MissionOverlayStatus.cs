using System;
using System.Collections.Generic;
using GemMatch;

namespace OverlayStatusSystem {
    public class MissionOverlayStatus : IOverlayStatus {
        private EntityModel _targetModel;
        public IOverlayStatusEvent EventListener { get; private set; }
        public Queue<OverlayStatusParam> EventRecord { get; private set; } = new Queue<OverlayStatusParam>();
        private event Action<EntityModel> OnMission;

        public MissionOverlayStatus(IOverlayStatusEvent missionStatusView, Action<EntityModel> onMission) {
            EventListener = missionStatusView;
            this.OnMission += onMission;
        }

        public MissionOverlayStatus SetTargetModel(EntityModel targetModel) {
            this._targetModel = targetModel;
            return this;
        }

        void IOverlayStatus.Save() {
            while (EventRecord.Count > 0) {
                OnMission?.Invoke(_targetModel);
            }
        }

        public void Enqueue(OverlayStatusParam inputParam) {
            EventRecord.Enqueue(inputParam);
        }
    }
}