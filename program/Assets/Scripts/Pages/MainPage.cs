using PagePopupSystem;
using UnityEngine;

namespace Pages {
    public class MainPage : PageHandler {
        public override Page GetPageType() => Page.MainPage;
        public void OnClickPlay() {
            Debug.Log($"Play");
        }
        
        private void Update() {
            if (Input.GetKeyDown(KeyCode.X)) {
                ChangeTo(Page.PlayPage);
            }
        }

    }
}