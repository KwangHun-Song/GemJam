using GemMatch;
using GemMatch.LevelEditor;
using PagePopupSystem;
using Popups;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pages {
    public class PlayPage : PageHandler {
        [SerializeField] private View view;
        
        public override Page GetPageType() => Page.PlayPage;
        public Controller Controller { get; private set; }

        public async void StartGame(int levelIndex) {
            Controller = new Controller();
            Controller.Listeners.Add(view);

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
                ChangeTo(Page.MainPage);
            }

            if (Input.GetKeyDown(KeyCode.P)) {
                var result =  await PopupManager.ShowAsync<bool>(nameof(ReadyPopup));
                Debug.Log(result);
            }

            if (Input.GetKeyDown(KeyCode.Escape)) {
                GoBackToEditMode();
            }
        }

        public void OnClickUndo() {
            if (Controller != null) {
                if (Controller.UndoHandler.IsEmpty()) {
                    Debug.Log($"Is Empty");
                    return;
                }
                Controller.UndoHandler.Undo();
            }
        }

        public void OnClickMagnet() {
            Controller?.InputAbility(new MagneticAbility(Controller));
        }

        public void OnClickShuffle() {
            Controller?.InputAbility(new ShuffleAbility(Controller));
        }

        private void GoBackToEditMode() {
            if (FindObjectOfType<EditLevelIndicator>() != null)
                SceneManager.LoadScene("EditScene");
        }

        #region CHEAT

        private int levelIndexInput;
        public string LevelIndexInput {
            set {
                if (int.TryParse(value, out var levelIndex)) {
                    levelIndexInput = levelIndex;
                }
            }
        }
        
        public void OnClickStartGame() {
            StartGame(levelIndexInput);    
        }

        #endregion
    }
}