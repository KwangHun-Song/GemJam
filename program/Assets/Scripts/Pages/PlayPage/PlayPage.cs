using System;
using System.Collections;
using System.ScreenLock;
using System.Threading.Tasks;
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
using Utility;

namespace Pages {
    public class PlayPageParam {
        public int levelIndex;
        public BoosterIndex[] selectedBoosters;
    }
    
    public class PlayPage : PageHandler {
        [SerializeField] private View[] views;
        [SerializeField] private PlayBoosterUI[] playBoosters;
        [SerializeField] private Animator clearRibbonAnimator;
        [SerializeField] private CharacterUI characterUi;

        public override SoundName BgmName => SoundName.bgm_play;
        public override Page GetPageType() => Page.PlayPage;
        public Controller Controller { get; private set; }
        public PlayPageParam Param { get; private set; }
        
        // 이 인덱스를 사용해 두 뷰를 번갈아가며 가져온다.
        private int currentViewIndex;
        private static readonly int On = Animator.StringToHash("on");
        private static readonly int Off = Animator.StringToHash("off");
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
            
            clearRibbonAnimator.gameObject.SetActive(false);
        }

        public void StartGame(int levelIndex, bool ignoreAnimation = false) {
            SimpleSound.PlayBGM(SoundName.bgm_play);
            Controller = new Controller();
            Controller.Listeners.Add(CurrentView);
            var level = LevelLoader.GetLevel(levelIndex);
            
            Controller.StartGame(level);
            if (ignoreAnimation) {
                ShowViewImmediately();
            } else {
                ShowViewMoveAnimationAsync().Forget();
            }
            
            WaitAndEndGameAsync().Forget();
        }

        public void ReplayGame() {
            Controller.ReplayGame();
            WaitAndEndGameAsync().Forget();
        }

        private void ShowViewImmediately() {
            (CurrentView.transform as RectTransform)!.anchoredPosition = Vector2.zero;
            (OtherView.transform as RectTransform)!.anchoredPosition = Vector2.left * (Screen.width + (76F * 4));
        }

        private async UniTask ShowViewMoveAnimationAsync() {
            var currentViewRectTfm = CurrentView.transform as RectTransform;
            var moveDistance = Screen.width + (76F * 4); // 대기하는 보드가 이전 보드를 가리지 않도록 패딩을 준다. 타일width x 4
            currentViewRectTfm!.anchoredPosition = Vector2.right * moveDistance;
            
            // 페이지 트랜지션 애니메이션이 진행중이면 잠시 대기한다.
            characterUi.WalkIn();
            await UniTask.WaitUntil(() => PageManager.OnTransitionAnimation == false);
            await UniTask.Delay(200);

            SimpleSound.Play(SoundName.line);
            currentViewRectTfm.DOAnchorPos(Vector2.zero, 0.4F).SetEase(Ease.OutBack);
            
            var otherViewRectTfm = OtherView.transform as RectTransform;
            if (otherViewRectTfm!.anchoredPosition.x < float.Epsilon) {
                otherViewRectTfm.DOAnchorPos(Vector2.left * moveDistance, 0.4F).SetEase(Ease.OutBack);
            }

            StopWalkingAsync().Forget();

            async UniTask StopWalkingAsync() {
                await UniTask.Delay(TimeSpan.FromSeconds(0.5F));
                characterUi.StopWalking();
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
                // 클리어 데이터 저장
                PlayerInfo.HighestClearedLevelIndex++;

                // 마지막 미션이 들어갈 때까지 잠시 딜레이
                using (new ScreenLock()) {
                    await UniTask.Delay(1500);
                
                    // 먼저 클리어리본을 보여준다.
                    characterUi.ClearPopup();
                    await ShowClearRibbonAsync();
                    characterUi.ShowGoal();
                
                    // 코인 생성 후 날아가는 연출을 대기한다. 그 후 클리어팝업을 띄운다.
                    await new ClearCoinAnimator().ShowCoinAnimation(CurrentView.TileViews);
                }
                
                var next = await PopupManager.ShowAsync<bool>(nameof(ClearPopup), Param.levelIndex + 1);

                if (next) {
                    currentViewIndex++;
                    Param.levelIndex = Mathf.Clamp(Param.levelIndex + 1, 0, LevelLoader.GetContainer().levels.Length - 1);
                    StartGame(Param.levelIndex);
                } else {
                    ChangeTo(Page.MainPage);
                }
            }

            if (gameResult == GameResult.Fail) {
                SimpleSound.StopBGM(0.5F);
                var failResult = await PopupManager.ShowAsync<FailPopupResult>(nameof(FailPopup), Param.levelIndex + 1);
                if (failResult?.isPlay ?? false) {
                    ReplayGame();
                    ApplyReadyBoosters(failResult.selectedBoosters);
                } else {
                    ChangeTo(Page.MainPage);
                }
            }
        }

        private async UniTask ShowClearRibbonAsync() {
            SimpleSound.StopBGM();
            SimpleSound.Play(SoundName.clear_ribbon);
            SimpleSound.Play(SoundName.clear_ribbon_firework);
            SimpleSound.Play(SoundName.uh_wow);
            clearRibbonAnimator.gameObject.SetActive(true);
            clearRibbonAnimator.SetTrigger(On);
            await clearRibbonAnimator.WaitForCurrentClip();
            clearRibbonAnimator.SetTrigger(Off);
            await clearRibbonAnimator.WaitForCurrentClip();
            clearRibbonAnimator.gameObject.SetActive(false);
        }

        #region PlayBooster
        public void UpdatePlayBooster() {
            foreach (var booster in playBoosters) {
                booster.Refresh();
            }
        }

        public void OnClickUndo() {
            SimpleSound.Play(SoundName.button_click);
            if (Controller != null) {
                if (Controller.UndoHandler.IsEmpty()) {
                    SimpleSound.Play(SoundName.a_ha);
                    ToastMessage.Show("There is nothing to undo.");
                    return;
                }
                Controller.UndoHandler.Undo();
            }
        }

        public void OnClickMagnet() {
            SimpleSound.Play(SoundName.button_click);
            Controller?.InputAbility(new MagneticAbility(Controller));
        }

        public void OnClickShuffle() {
            SimpleSound.Play(SoundName.button_click);
            SimpleSound.Play(SoundName.shuffle);
            Controller?.InputAbility(new ShuffleAbility(Controller));
        }

        public void OnClickSetting() {
            SimpleSound.Play(SoundName.button_click);
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
                if (FindObjectOfType<EditPage>(true) != null)
                    ChangeTo(Page.EditPage);
                else
                    ChangeTo(Page.MainPage);
            }
        }

        #region CHEAT

        public void OnClickPrev() {
            StartGame(--Param.levelIndex, true);
        }

        public void OnClickNext() {
            StartGame(++Param.levelIndex, true);
        }
        
        public void OnClickStartGame() {
            StartGame(Param.levelIndex, true);    
        }

        #endregion
    }
}