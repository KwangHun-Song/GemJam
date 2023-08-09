using System;
using System.Collections.Generic;
using Record;

namespace OverlayStatusSystem {
    public class CoinOverlayStatus : IOverlayStatus {
        private int _coinCount;
        public IOverlayStatusEvent EventListener { get; private set; }
        public Queue<OverlayStatusParam> EventRecord { get; private set; } = new Queue<OverlayStatusParam>();
        private event Action<int> OnCoin;

        public CoinOverlayStatus(IOverlayStatusEvent coinStatusView, Action<int> onCoin) {
            EventListener = coinStatusView;
            this.OnCoin += onCoin;
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
        }
    }
}