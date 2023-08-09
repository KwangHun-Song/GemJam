using System;
using Cysharp.Threading.Tasks;
using Record;
using UnityEngine;

namespace OverlayStatusSystem {
    public class LevelStatusView : OverlayStatusEvent<LevelOverlayStatus> {
        [SerializeField] private GameObject[] stageIndexers;
        private int stageIndex => PlayerInfo.HighestClearedLevelIndex % 3;

        public void Start() {
            OverlayStatusHelper.Init(new LevelOverlayStatus(this, OnStage));
        }

        private void OnStage(int count) {
            TurnOnStageIndex();
        }

        private void TurnOnStageIndex() {
            for (int i = 0; i < stageIndexers.Length; i++) {
                var cursor = stageIndexers[i];
                cursor.SetActive(i <= stageIndex);
            }
        }

        public async UniTaskVoid GetStage() {
            await base.Get<int>(1);
        }

        public override UniTask Animate() {
            return base.Animate(); // todo: 연출을 여기 넣는다
        }
    }

    public class LevelOverlayStatus : OverlayStatus<int> {
        public LevelOverlayStatus(IOverlayStatusEvent levelStatusView, Action<int> onStage) : base (levelStatusView, onStage) {
        }

        public override void Save() {
            while (EventRecord.Count > 0) {
                EventRecord.Dequeue();
                OnEvent?.Invoke(1); // stage는 1개씩 클리어 고정
            }
        }
    }
}