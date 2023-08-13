using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utility;

namespace GemMatch {
    public class NormalPieceView : EntityView, IReceiveActivation, IPointerEnterHandler, IPointerExitHandler {
        [Header("빨, 주, 노, 초, 파, 보 순서로 넣어주세요.")]
        [SerializeField] private Sprite[] sprites;
        [SerializeField] private Sprite[] dimSprites;
        [SerializeField] private Sprite[] slotSprites;
        [SerializeField] private Image mainImage;
        [SerializeField] private Image dimImage;
        [SerializeField] private Image slotImage;
        [SerializeField] private Animator animator;
        
        private static readonly int Idle = Animator.StringToHash("idle");
        private static readonly int On = Animator.StringToHash("on");
        private static readonly int Off = Animator.StringToHash("off");
        private static readonly int Crash = Animator.StringToHash("crash");
        
        public bool OnCrashAnimation { get; private set; }
        private bool OnShowingActive { get; set; }

        public async override UniTask OnCreate() {
            Redraw();
            SetOnMemoryUI(false);
            SetCanTouchUI(false);
            transform.localScale = Vector3.one;
            animator.SetTrigger(Idle);
        }

        public async override UniTask OnUpdate() {
            Redraw();
        }

        public void SetCanTouchUI(bool canTouch) {
            mainImage.gameObject.SetActive(canTouch);
            dimImage.gameObject.SetActive(!canTouch);
        }

        public void SetOnMemoryUI(bool onMemory) {
            mainImage.gameObject.SetActive(!onMemory);
            dimImage.gameObject.SetActive(!onMemory);
            slotImage.gameObject.SetActive(onMemory);
        }

        public void Redraw() {
            if (Entity.Color != ColorIndex.Random)
                if (Constants.UsableColors.Contains(Entity.Color) == false) return;
            var index = Entity.Color == ColorIndex.Random ? sprites.Length-1 : (int)Entity.Color;
            mainImage.sprite = sprites[index];
            dimImage.sprite = dimSprites[index];
            slotImage.sprite = slotSprites[index];
        }

        public void OnClick() {
            TileView.View.OnClickEntity(Entity);
        }

        public override async UniTask OnMoveMemory() {
            SimpleSound.Play(SoundName.piece_touch);
            OnCrashAnimation = true;
            await transform.DOScale(Vector3.zero, 0.15F).SetEase(Ease.OutQuad).ToUniTask();
            SetOnMemoryUI(true);
        }

        public async UniTask OnActive(bool isActive) {
            OnShowingActive = isActive;
            SetOnMemoryUI(false);
            SetCanTouchUI(isActive || OnCrashAnimation);
        }

        public void OnPointerEnter(PointerEventData eventData) {
            if (OnShowingActive == false) return;
            animator.SetTrigger(On);
        }

        public void OnPointerExit(PointerEventData eventData) {
            animator.SetTrigger(Off);
        }

        public override async UniTask DestroyAsync(bool isImmediately = false) {
            if (isImmediately) {
                DestroyImmediate(gameObject);
                return;
            }
            
            animator.SetTrigger(Crash);
            
            var effectName = $"CrashNormalPiece{Entity.Color.ToString().Substring(0, 1)}";
            var crashEffect = Instantiate(Resources.Load<GameObject>(effectName), transform.parent);
            crashEffect.transform.position = transform.position;
            
            DestroyAfterAnimation().Forget();
            DestroyEffectAsync().Forget();

            async UniTask DestroyAfterAnimation() {
                await animator.WaitForCurrentClip();
                DestroyImmediate(gameObject);
            }

            async UniTask DestroyEffectAsync() {
                await UniTask.Delay(2000);
                DestroyImmediate(crashEffect.gameObject);
            }
        }

        public void SetCrashAnimationEnd() { }
    }
}