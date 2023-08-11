using System.Collections.Generic;
using GemMatch;
using PagePopupSystem;
using Record;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Popups {
    public class FailPopupResult {
        public BoosterIndex[] selectedBoosters;
        public bool isPlay;
    }
    
    public class FailPopup : PopupHandler {
        [SerializeField] private Toggle[] boosters;
        [SerializeField] private TMP_Text titleText;

        private List<BoosterIndex> selectedBoosters = new List<BoosterIndex>();

        public bool IsSelectRocket {
            set {
                if (value) selectedBoosters.Add(BoosterIndex.ReadyBoosterRocket);
                else selectedBoosters.Remove(BoosterIndex.ReadyBoosterRocket);
            }
        }

        public bool IsSelectExtraSlot {
            set {
                if (value) selectedBoosters.Add(BoosterIndex.ReadyBoosterExtraSlot);
                else selectedBoosters.Remove(BoosterIndex.ReadyBoosterExtraSlot);
            }
        }

        public override void OnWillEnter(object param) {
            // UI 초기화시 토글은 해제되어 있음
            foreach (var booster in boosters) {
                booster.isOn = false;
            }
            selectedBoosters.Clear();
            titleText.text = $"Level {(int)param}";
        }

        public void OnClickPlay() {
            Close(new FailPopupResult { selectedBoosters = selectedBoosters.ToArray(), isPlay = true });
        }
    }
}