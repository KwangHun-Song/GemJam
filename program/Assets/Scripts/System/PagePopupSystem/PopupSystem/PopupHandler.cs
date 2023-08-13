using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using GemMatch;
using UnityEngine;
using Utility;

namespace PagePopupSystem {
    public abstract class PopupHandler : MonoBehaviour {
        [SerializeField] private Transform animationTarget;
        
        public virtual void OnWillEnter(object param) { }

        public virtual UniTask OnDidEnter(object param) => UniTask.CompletedTask;
        public virtual void OnWillLeave() { }
        public virtual void OnDidLeave() { }

        public virtual void OnClickOk() {
            SimpleSound.Play(SoundName.button_click);
            HideWithAnimation(true).Forget();
        }

        public virtual void OnClickClose() {
            SimpleSound.Play(SoundName.button_click);
            HideWithAnimation(false).Forget();
        }

        internal UniTaskCompletionSource<object> popupTask;

        public string Name => GetType().Name;

        private Canvas canvas;
        private Canvas Canvas => canvas ??= GetComponentInChildren<Canvas>(true);

        internal void SetSortingOrder(int sortingOrder) => Canvas.sortingOrder = sortingOrder;

        internal int GetSortingOrder() => Canvas.sortingOrder;

        internal async UniTask ShowWithAnimation(object param) {
            if (gameObject.activeSelf) {
                throw new InvalidOperationException("The popup is already active.");
            }

            gameObject.SetActive(true);
            animationTarget.localScale = Vector3.zero;
            OnWillEnter(param);
            await animationTarget.DOScale(Vector3.one, 0.3F).SetEase(Ease.OutBack).ToUniTask();
            OnDidEnter(param).Forget();

            popupTask = new UniTaskCompletionSource<object>();
        }

        public void Close(object param) => HideWithAnimation(param).Forget();

        internal async UniTask HideWithAnimation(object param) {
            if (!gameObject.activeSelf) {
                throw new InvalidOperationException("The popup is already hidden.");
            }

            OnWillLeave();
            SimpleSound.Play(SoundName.popup_out);
            await animationTarget.DOScale(Vector3.zero, 0.15F).SetEase(Ease.InBack).ToUniTask();
            OnDidLeave();

            gameObject.SetActive(false);

            if (popupTask != null) {
                popupTask.TrySetResult(param);
            } else {
                Debug.LogWarning("Popup task is not initialized.");
            }
        }
    }
}