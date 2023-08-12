using Cysharp.Threading.Tasks;
using PagePopupSystem;
using Popups;
using Record;
using ToastMessageSystem;
using UnityEngine;
using Utility.CustomMouse;
using UnityEngine.SceneManagement;

namespace Pages {
    public class MainPage : PageHandler {
        public override Page GetPageType() => Page.MainPage;

        #region EVENT

        public void OnClickCoinStatusBar() {
            ToastMessage.Show("It will be implemented soon.");
        }

        public void OnClickSetting() {
            ToastMessage.Show("It will be implemented soon.");
        }
        
        public void OnClickPlay() {
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

#if UNITY_EDITOR
        private void Update() {
            if (Input.GetKeyDown(KeyCode.F10)) {
                CustomMouse.Show();
            }

            if (Input.GetKeyDown(KeyCode.E)) {
                SceneManager.LoadScene("EditScene");
            }
        }
#endif
    }
}