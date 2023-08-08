using GemMatch;
using PagePopupSystem;
using Popups;
using UnityEngine;

namespace Pages {
    public class PlayPage : PageHandler {
        [SerializeField] private View view;
        
        public Controller Controller { get; private set; }

        [ContextMenu("Start")]
        public async void StartGame() {
            Controller = new Controller();
            Controller.Listeners.Add(view);
            
            var level = LevelLoader.GetLevel(0);
            
            Controller.StartGame(level);

            var result = await Controller.WaitUntilGameEnd();
            
            Debug.Log(result);
        }

        private async void Update() {
            if (Input.GetKeyDown(KeyCode.O)) {
                FadeOutHelper.FadeOut();
            } else if (Input.GetKeyDown(KeyCode.I)) {
                FadeOutHelper.FadeIn();
            }

            if (Input.GetKeyDown(KeyCode.X)) {
                ChangeTo(nameof(MainPage));
            }

            if (Input.GetKeyDown(KeyCode.P)) {
                var result =  await PopupManager.ShowAsync<bool>(nameof(ReadyPopup));
                Debug.Log(result);
            }
        }
    }
}