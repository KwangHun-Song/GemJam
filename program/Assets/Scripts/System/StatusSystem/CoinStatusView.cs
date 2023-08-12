using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Record;
using TMPro;
using UnityEngine;

namespace OverlayStatusSystem {
    public class CoinStatusView : OverlayStatusEvent<CoinOverlayStatus> {
        [SerializeField] private TMP_Text coin;
        public Transform CoinRoot;

        private void Start() {
            OverlayStatusHelper.Init(new CoinOverlayStatus(this, OnCoin));
            coin.text = $"{Wallet.GetItemCount(Item.Coin)}";
        }

        public void InputCoin(int amount) {
            OverlayStatusHelper.Input(this, new OverlayStatusParam(amount));
        }

        public async UniTaskVoid GetCoin() {
            OverlayStatusHelper.Save(this);
        }

        private void OnCoin(int value) {
            if (int.TryParse(coin.text, out int coinCache)) {
                DOTween.To(() => coinCache, x => coinCache = x, value, 0.5f).OnUpdate(() => coin.text = $"{coinCache}");
                return;
            }

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