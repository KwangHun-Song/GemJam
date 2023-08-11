using PagePopupSystem;
using TMPro;
using UnityEngine;

namespace Popups {
    public class ClearPopup : PopupHandler {
        [SerializeField] private TMP_Text titleText;

        public override void OnWillEnter(object param) {
            titleText.text = $"Level {(int)param}";
        }
    }
}