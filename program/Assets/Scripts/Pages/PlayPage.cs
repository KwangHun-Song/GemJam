using System.Collections;
using Cysharp.Threading.Tasks;
using GemMatch;
using GemMatch.LevelEditor;
using PagePopupSystem;
using Popups;
using Record;
using ToastMessageSystem;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace Pages {
    public class PlayPageParam {
        public int levelIndex;
        public BoosterIndex[] selectedBoosters;
    }
    
    public class PlayPage : PageHandler {
        [SerializeField] private View view;
        [SerializeField] private PlayBoosterUI[] playBoosters;
        
        public override Page GetPageType() => Page.PlayPage;
        public Controller Controller { get; private set; }
        public PlayPageParam Param { get; private set; }

        public override void OnWillEnter(object param) {
            Assert.IsTrue(param is PlayPageParam);
            Param = (PlayPageParam)param;

            if (FindObjectOfType<EditLevelIndicator>() is EditLevelIndicator indicator && indicator != null) {
                Param.levelIndex = indicator.LevelIndex;
            }
            
            StartGame(Param.levelIndex);
            ApplyReadyBoosters(Param.selectedBoosters);
            WaitAndEndGameAsync().Forget();
        }

        public Controller StartGame(int levelIndex) {
            Controller = new Controller();
            Controller.Listeners.Add(view);
            var level = LevelLoader.GetLevel(levelIndex);
            
            Controller.StartGame(level);
            return Controller;
        }

        private void ApplyReadyBoosters(BoosterIndex[] selectedBoosters) {
            // Controller.StartGame이 되고난 후 실행해주세요.
            foreach (var selectedBooster in selectedBoosters) {
                switch (selectedBooster) {
                    case BoosterIndex.ReadyBoosterRocket:
                        Controller.InputAbility(new RocketAbility(Controller));
                        Controller.UndoHandler.Reset();
                        break;
                    case BoosterIndex.ReadyBoosterExtraSlot:
                        Controller.AddExtraMemorySlot();
                        break;
                }
            }
        }

        private async UniTask WaitAndEndGameAsync() {
            await UniTask.Yield();
            var gameResult = await Controller.WaitUntilGameEnd();
            if (gameResult == GameResult.Clear) {
                var next = await PopupManager.ShowAsync<bool>(nameof(ClearPopup), Param.levelIndex + 1);

                // 클리어 데이터 저장
                PlayerInfo.HighestClearedLevelIndex++;

                if (next) {
                    Param.levelIndex = Mathf.Clamp(Param.levelIndex + 1, 0, LevelLoader.GetContainer().levels.Length - 1);
                    StartGame(Param.levelIndex);
                } else {
                    ChangeTo(Page.MainPage);
                }
            }

            if (gameResult == GameResult.Fail) {
                var failResult = await PopupManager.ShowAsync<FailPopupResult>(nameof(FailPopup), Param.levelIndex + 1);
                if (failResult.isPlay) {
                    Controller.ReplayGame();
                    ApplyReadyBoosters(failResult.selectedBoosters);
                    WaitAndEndGameAsync().Forget();
                } else {
                    ChangeTo(Page.MainPage);
                }
            }
        }

        #region PlayBooster
        public void UpdatePlayBooster() {
            foreach (var booster in playBoosters) {
                booster.Refresh();
            }
        }

        public void OnClickUndo() {
            if (Controller != null) {
                if (Controller.UndoHandler.IsEmpty()) {
                    ToastMessage.Show("There is nothing to undo.");
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

        public void OnClickSetting() {
            ChangeTo(Page.MainPage);
        }

        #endregion // PlayBooster

        private IEnumerator Start() {
            yield return null;
            yield return null;
            GetStable();
        }

        private void GetStable() {
            UpdatePlayBooster();
        }

        private void Update() {
            if (Input.GetKeyUp(KeyCode.N)) {
                PlayerInfo.HighestClearedLevelIndex = 0;
                OnClickStartGame();
            }

            if (Input.GetKeyDown(KeyCode.C)) {
                Controller?.ClearGame();
            }

            if (Input.GetKeyDown(KeyCode.F)) {
                Controller?.FailGame();
            }

            if (Input.GetKeyDown(KeyCode.E)) {
                SceneManager.LoadScene("EditScene");
            }
            
            if (Input.GetKeyDown(KeyCode.Escape)) {
                if (FindObjectOfType<EditPage>(true) != null) return;
                ChangeTo(Page.MainPage);
            }
        }
        

        private void GoBackToEditMode() {
            Destroy(this.gameObject);
        }

        #region CHEAT

        public void OnClickPrev() {
            StartGame(--Param.levelIndex);
        }

        public void OnClickNext() {
            StartGame(++Param.levelIndex);
        }
        
        public void OnClickStartGame() {
            StartGame(Param.levelIndex);    
        }

        #endregion
    }
}