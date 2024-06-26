using System.Collections.Generic;
using GemMatch;
using PagePopupSystem;
using Record;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using Utility;

namespace Popups {
    public class ReadyPopupResult {
        public BoosterIndex[] selectedBoosters;
        public bool isPlay;
    }
    
    public class ReadyPopup : PopupHandler {
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
            Assert.IsTrue(param is int);
            // UI 초기화시 토글은 해제되어 있음
            foreach (var booster in boosters) {
                booster.isOn = false;
            }
            selectedBoosters.Clear();
            titleText.text = $"Level {(int)param + 1}";
        }

        public void OnClickPlay() {
            SimpleSound.Play(SoundName.button_click);
            Close(new ReadyPopupResult { selectedBoosters = selectedBoosters.ToArray(), isPlay = true });
        }
    }
}