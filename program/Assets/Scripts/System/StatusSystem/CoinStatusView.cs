using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace OverlayStatusSystem {
    /// <summary>
    /// canvas에 들어갈 클래스
    /// </summary>
    public class CoinStatusView : MonoBehaviour, IOverlayStatusEvent {
        [SerializeField] private TMP_Text coin;

        private void Start() {
            OverlayStatusHelper.Init(new CoinOverlayStatus(this, OnCoin));
            // go를 키로 coinparam을 모아서(list에 저장, input마다) save(go를 키)할때 저장 record에 걸린 값으로 트리거
        }

        public async UniTaskVoid GetCoin(int amount) {
            OverlayStatusHelper.Input(this, new OverlayStatusParam(amount));
            await UniTask.Delay(1000); // 몬가 애니메이션 연출을 여기 넣는다
            OverlayStatusHelper.Save(this);
        }

        public MonoBehaviour Mono => this;

        private void OnCoin(int value) {
            coin.text = $"{value}";
        }

        public Type GetType() {
            return typeof(CoinOverlayStatus);
        }
    }
}