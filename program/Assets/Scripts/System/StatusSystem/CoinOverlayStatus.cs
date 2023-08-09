using System;
using System.Collections.Generic;
using UnityEngine;

namespace OverlayStatusSystem {
    public class CoinOverlayStatus : IOverlayStatus {
        private const string CoinPrefsKey = "CoinStatus";
        private int _coinCount;
        public IOverlayStatusEvent EventListener { get; private set; }
        public Queue<OverlayStatusParam> EventRecord { get; private set; } = new Queue<OverlayStatusParam>();

        public CoinOverlayStatus(IOverlayStatusEvent coinStatusView, Action<int> onCoin) {
            EventListener = coinStatusView;
            this.OnCoin += onCoin;
        }

        private event Action<int> OnCoin;

        void IOverlayStatus.Save() {
            int newCoin = 0;
            while (EventRecord.Count > 0) {
                newCoin += (int)EventRecord.Dequeue().Value;
            }
            PlayerPrefs.SetInt(CoinPrefsKey, PlayerPrefs.GetInt(CoinPrefsKey, 0) + newCoin);
            OnCoin?.Invoke(PlayerPrefs.GetInt(CoinPrefsKey, 0));
        }

        public void Enqueue(OverlayStatusParam inputParam) {
            EventRecord.Enqueue(inputParam);
        }
    }
}