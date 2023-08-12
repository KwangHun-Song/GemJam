using System.Collections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
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
        [SerializeField] private View[] views;
        [SerializeField] private PlayBoosterUI[] playBoosters;
        [SerializeField] private Transform coinAnimationTarget;
        
        public override Page GetPageType() => Page.PlayPage;
        public Controller Controller { get; private set; }
        public PlayPageParam Param { get; private set; }
        
        // 이 인덱스를 사용해 두 뷰를 번갈아가며 가져온다.
        private int currentViewIndex;
        public int CurrentViewIndex => currentViewIndex % 2;
        public int OtherViewIndex => (currentViewIndex + 1) % 2;

        public View CurrentView => views[CurrentViewIndex];
        public View OtherView => views[OtherViewIndex];

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

        public void StartGame(int levelIndex) {
            Controller = new Controller();
            Controller.Listeners.Add(CurrentView);
            var level = LevelLoader.GetLevel(levelIndex);
            
            Controller.StartGame(level);
            ShowViewMoveAnimation();
        }

        private void ShowViewMoveAnimation() {
            var currentViewRectTfm = CurrentView.transform as RectTransform;
            var moveDistance = Screen.width + (76F * 4); // 대기하는 보드가 이전 보드를 가리지 않도록 패딩을 준다.
            currentViewRectTfm!.anchoredPosition = Vector2.right * moveDistance;
            currentViewRectTfm.DOAnchorPos(Vector2.zero, 0.4F).SetEase(Ease.OutBack).SetDelay(0.8F);
            
            var otherViewRectTfm = OtherView.transform as RectTransform;
            if (otherViewRectTfm!.anchoredPosition.x < float.Epsilon) {
                otherViewRectTfm.DOAnchorPos(Vector2.left * moveDistance, 0.4F).SetEase(Ease.OutBack).SetDelay(0.8F);
            }
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
                // 코인 생성 후 날아가는 연출을 대기한다. 그 후 클리어팝업을 띄운다.
                await new ClearCoinAnimator().ShowCoinAnimation(CurrentView.TileViews, coinAnimationTarget);
                
                var next = await PopupManager.ShowAsync<bool>(nameof(ClearPopup), Param.levelIndex + 1);

                // 클리어 데이터 저장
                PlayerInfo.HighestClearedLevelIndex++;

                if (next) {
                    currentViewIndex++;
                    Param.levelIndex = Mathf.Clamp(Param.levelIndex + 1, 0, LevelLoader.GetContainer().levels.Length - 1);
                    StartGame(Param.levelIndex);
                    WaitAndEndGameAsync().Forget();
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