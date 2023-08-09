using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace OverlayStatusSystem {
    public class LevelStatusView : OverlayStatusEvent<LevelOverlayStatus> {
        [SerializeField] private GameObject[] stageIndexers;

        public async UniTaskVoid GetStage() {
            await base.Get<int>(1);
        }

        public override UniTask Animate() {
            return base.Animate();
        }
    }

    public class LevelOverlayStatus : IOverlayStatus {
        public IOverlayStatusEvent EventListener { get; private set; }
        public Queue<OverlayStatusParam> EventRecord { get; private set; } = new Queue<OverlayStatusParam>();
        private event Action<int> OnStage;

        public LevelOverlayStatus(IOverlayStatusEvent coinStatusView, Action<int> onStage) {
            EventListener = coinStatusView;
            this.OnStage += onStage;
        }

        void IOverlayStatus.Save() {
            int newCoin = 0;
            while (EventRecord.Count > 0) {
                newCoin += (int)EventRecord.Dequeue().Value;
            }
            Wallet.Gain(Item.Coin, newCoin);
            OnCoin?.Invoke(Wallet.GetItemCount(Item.Coin));
        }

        public void Enqueue(OverlayStatusParam inputParam) {
            EventRecord.Enqueue(inputParam);
        }드
    }

    public class OverlayStatusEvent<Status> : MonoBehaviour, IOverlayStatusEvent where Status : IOverlayStatus {
        public Type GetKeyType() => typeof(Status);

        public virtual async UniTask Get<T>(T amount) {
            OverlayStatusHelper.Input(this, new OverlayStatusParam(amount));
            await Animate();
            OverlayStatusHelper.Save(this);
        }

        public virtual UniTask Animate() {
            // 몬가 애니메이션 연출을 여기 넣는다
            return UniTask.Delay(1000);
        }
    }
}