using UnityEngine;

namespace PagePopupSystem {
    public abstract class PageHandler : MonoBehaviour {
        public virtual void OnWillEnter(object param) { }

        public string Name => GetType().Name;

        public void ChangeTo(string pageName, object inParam = null) {
            PageManager.ChangeTo(pageName, inParam, gameObject).Forget();
        }
    }
}