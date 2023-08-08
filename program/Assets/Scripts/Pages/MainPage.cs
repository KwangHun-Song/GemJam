using PagePopupSystem;
using UnityEngine;

namespace Pages {
    public class MainPage : PageHandler {
        private void Update() {
            if (Input.GetKeyDown(KeyCode.X)) {
                ChangeTo(nameof(PlayPage));
            }
        }
    }
}