using Cysharp.Threading.Tasks;
using PagePopupSystem;
using Popups;
using Record;

namespace Pages {
    public class MainPage : PageHandler {
        public override Page GetPageType() => Page.MainPage;
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
    }
}