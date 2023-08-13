using PagePopupSystem;
using TMPro;
using UnityEngine;
using Utility;

namespace Popups {
    public class ClearPopup : PopupHandler {
        [SerializeField] private TMP_Text titleText;

        public override void OnWillEnter(object param) {
            titleText.text = $"Level {(int)param}";
            SimpleSound.Play(SoundName.clearpopup);
        }
    }
}