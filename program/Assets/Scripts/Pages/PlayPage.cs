using GemMatch;
using GemMatch.LevelEditor;
using PagePopupSystem;
using Popups;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pages {
    public class PlayPage : PageHandler {
        [SerializeField] private View view;
        
        public Controller Controller { get; private set; }

        [ContextMenu("Start")]
        public async void StartGame() {
            Controller = new Controller();
            Controller.Listeners.Add(view);

            var levelIndex = 0;
            if (FindObjectOfType<EditLevelIndicator>() is EditLevelIndicator indicator && indicator != null) {
                levelIndex = indicator.LevelIndex;
            }
            var level = LevelLoader.GetLevel(levelIndex);
            
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

            if (Input.GetKeyDown(KeyCode.Escape)) {
                GoBackToEditMode();
            }
        }

        private void GoBackToEditMode() {
            if (FindObjectOfType<EditLevelIndicator>() != null)
                SceneManager.LoadScene("EditScene");
        }
    }
}