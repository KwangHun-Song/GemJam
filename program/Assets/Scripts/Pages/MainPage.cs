using PagePopupSystem;

namespace Pages {
    public class MainPage : PageHandler {
        public override Page GetPageType() => Page.MainPage;
        public void OnClickPlay() {
            ChangeTo(Page.PlayPage);
        }
    }
}