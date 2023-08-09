using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Utility {
    /// <summary>
    /// Will scale uiItem up and down, on press events
    /// </summary>
    public class ButtonScaleTransition : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler {
        private Vector3 onUpScale; //keeps track of original scale
        private Button button; //keeps track of button component
        
        [SerializeField] private Vector3 onDownScale = new Vector3(0.94f, 0.94f, 0.94f);
        [SerializeField] private float tweenDuration = 0.05f;
        [SerializeField] private bool lockScale = true;
        [SerializeField] private Transform overrideTargetTransform = null;

        public float TweenDuration => tweenDuration;

        private Coroutine scaleTweenCoroutine;
        private Transform targetTransform;

        private void Awake() {
            targetTransform = overrideTargetTransform != null ? overrideTargetTransform : transform;
            onUpScale = targetTransform.localScale;
            button = GetComponent<Button>();
            
            // 커스텀 트랜지션을 넣으니 기존 트랜지션은 None으로 설정한다.
            button.transition = Selectable.Transition.None;
        }

        private void OnEnable() {
            if (lockScale) targetTransform.localScale = onUpScale;
        }

        private void OnDisable() {
            StopAllCoroutines();
        }

        private void ButtonDown() {
            Vector3 localScale = targetTransform.localScale;
            if (!lockScale) onUpScale = localScale;
            localScale.Scale(onDownScale);

            if (tweenDuration <= 0) {
                targetTransform.localScale = localScale;
            } else {
                var transform1 = targetTransform;
                transform1.localScale = onUpScale;
                StartScaleTween(transform1.localScale, localScale);
            }
        }

        public void StartScaleTween(Vector3 tweenStartingScale, Vector3 tweenTargetScale) {
            if (scaleTweenCoroutine != null) {
                StopCoroutine(scaleTweenCoroutine);
            }

            scaleTweenCoroutine = StartCoroutine(ScaleTween(tweenStartingScale, tweenTargetScale));
        }

        private void ButtonUp() {
            if (tweenDuration <= 0) {
                targetTransform.localScale = onUpScale;
            } else {
                StartScaleTween(targetTransform.localScale, onUpScale);
            }
        }

        private IEnumerator ScaleTween(Vector3 tweenStartingScale, Vector3 tweenTargetScale) {
            var tweenTimeElapsed = 0.0F;
            while (tweenTimeElapsed < tweenDuration) {
                targetTransform.localScale =
                    Vector3.Lerp(tweenStartingScale, tweenTargetScale, tweenTimeElapsed / tweenDuration);
                yield return null;
                tweenTimeElapsed += Time.deltaTime;
            }

            targetTransform.localScale = tweenTargetScale;
            if (!lockScale) onUpScale = targetTransform.localScale;
        }

        public void OnPointerDown(PointerEventData eventData) {
            if (IsAnimationBlocked()) return;
            ButtonDown();
        }

        public void OnPointerUp(PointerEventData eventData) {
            if (IsAnimationBlocked()) return;
            ButtonUp();
        }

        public void OnPointerExit(PointerEventData eventData) {
            if (IsAnimationBlocked()) return;
            ButtonUp();
        }

        private bool IsAnimationBlocked() {
            return button && !button.interactable;
        }
    }
}