using Cysharp.Threading.Tasks;
using PagePopupSystem;
using Popups;
using Record;
using ToastMessageSystem;
using UnityEngine;
using Utility.CustomMouse;
using UnityEngine.SceneManagement;
using Utility;

namespace Pages {
    public class MainPage : PageHandler {
        public override SoundName BgmName => SoundName.bgm_main;
        public override Page GetPageType() => Page.MainPage;

        #region EVENT

        public void OnClickCoinStatusBar() {
            SimpleSound.Play(SoundName.a_ha);
            ToastMessage.Show("It will be implemented soon.");
        }

        public void OnClickSetting() {
            SimpleSound.Play(SoundName.button_click);
            SimpleSound.Play(SoundName.a_ha);
            ToastMessage.Show("It will be implemented soon.");
        }
        
        public void OnClickPlay() {
            SimpleSound.Play(SoundName.button_click);
            ClickPlayAsync().Forget();

            async UniTask ClickPlayAsync() {
                var levelIndexToPlay = PlayerInfo.HighestClearedLevelIndex + 1;
                var readyPopupResult = await PopupManager.ShowAsync<ReadyPopupResult>(nameof(ReadyPopup), levelIndexToPlay);

                if (readyPopupResult?.isPlay ?? false) {
                    ChangeTo(Page.PlayPage, new PlayPageParam {
                        levelIndex = levelIndexToPlay, 
                        selectedBoosters = readyPopupResult.selectedBoosters
                    });
                }
            }
        }

        #endregion

        private void Update() {
            if (Input.GetKeyDown(KeyCode.F10)) {
                CustomMouse.Show();
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                Time.timeScale = Time.timeScale > 1 ? 1 : 0.1F;
            }

            if (Input.GetKeyDown(KeyCode.RightArrow)) {
                Time.timeScale = Time.timeScale < 1 ? 1 : 10F;
            }

#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.E)) {
                SceneManager.LoadScene("EditScene");
            }
#endif
        }
    }
}