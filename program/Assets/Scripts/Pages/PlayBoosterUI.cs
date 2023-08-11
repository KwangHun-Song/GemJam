using System;
using Record;
using TMPro;
using UnityEngine;

namespace Pages {
    public enum PlayBoosterType {
        Undo, Magnet, Shuffle, Rocket,
    }
    public class PlayBoosterUI : MonoBehaviour{
        [SerializeField] private GameObject goIcon;
        [SerializeField] private GameObject goLock;
        [SerializeField] private TMP_Text txtUnlockLevel;
        [SerializeField] private TMP_Text txtItemCount;
        [SerializeField] private TMP_Text txtPrice;
        [SerializeField] private PlayBoosterType boosterType;

        private Record.Item GetItemMapping() => boosterType switch {
            PlayBoosterType.Undo => Record.Item.PlayUndo,
            PlayBoosterType.Magnet => Record.Item.PlayMagnet,
            PlayBoosterType.Shuffle => Record.Item.PlayShuffle,
            PlayBoosterType.Rocket => Record.Item.ReadyRocket,
            _ => throw new ArgumentOutOfRangeException()
        };

        public void Refresh() {
            // todo: 추후 조건 주가
            int unlockLevel = boosterType switch {
                PlayBoosterType.Rocket => 8,
                _ => 0,
            };
            txtUnlockLevel.text = $"Lv.{unlockLevel}";
            goIcon.SetActive(PlayerInfo.HighestClearedLevelIndex >= unlockLevel);
            goLock.SetActive(PlayerInfo.HighestClearedLevelIndex < unlockLevel);
            txtItemCount.text = $"{Wallet.GetItemCount(GetItemMapping())}";
            txtPrice.text = $"{Wallet.GetItemPrice(GetItemMapping())}";
        }
    }
}