using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace OverlayStatusSystem {
    public class LevelStatusView : OverlayStatusEvent<LevelOverlayStatus> {
        [SerializeField] private GameObject[] stageIndexers;

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
            int newCoin = 0;
            while (EventRecord.Count > 0) {
                newCoin += (int)EventRecord.Dequeue().Value;
            }
            Wallet.Gain(Item.Coin, newCoin);
            OnCoin?.Invoke(Wallet.GetItemCount(Item.Coin));
        }
    }
}