using System;
using Cysharp.Threading.Tasks;
using Record;
using TMPro;
using UnityEngine;

namespace OverlayStatusSystem {
    /// <summary>
    /// canvas에 들어갈 클래스
    /// </summary>
    public class CoinStatusView : OverlayStatusEvent<CoinOverlayStatus> {
        [SerializeField] private TMP_Text coin;

        private void Start() {
            OverlayStatusHelper.Init(new CoinOverlayStatus(this, OnCoin));
            // go를 키로 coinparam을 모아서(list에 저장, input마다) save(go를 키)할때 저장 record에 걸린 값으로 트리거
        }

        public async UniTaskVoid GetCoin(int amount) {
            await Get<int>(amount);
        }

        public override UniTask Animate() {
            return base.Animate();  // 몬가 애니메이션 연출을 여기 넣는다
        }

        private void OnCoin(int value) {
            coin.text = $"{value}";
        }
    }

    public class CoinOverlayStatus : OverlayStatus<int> {
        public CoinOverlayStatus(IOverlayStatusEvent statusView, Action<int> onCoin) : base(statusView, onCoin) {
        }

        public override void Save() {
            int newCoin = 0;
            while (EventRecord.Count > 0) {
                newCoin += (int)EventRecord.Dequeue().Value;
            }
            Wallet.Gain(Item.Coin, newCoin);
            base.OnEvent?.Invoke(Wallet.GetItemCount(Item.Coin));
        }
    }
}