using UnityEngine;

namespace PagePopupSystem {
    public abstract class PageHandler : MonoBehaviour {
        public virtual void OnWillEnter(object param) { }

        public abstract Page GetPageType();

        public void ChangeTo(Page pageType, object inParam = null) {
            PageManager.ChangeTo(pageType, inParam).Forget();
        }
    }
}