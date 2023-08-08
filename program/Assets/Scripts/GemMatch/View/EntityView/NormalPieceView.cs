using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace GemMatch {
    public class NormalPieceView : EntityView, IReceiveActivation {
        [Header("빨, 주, 노, 초, 파, 보 순서로 넣어주세요.")]
        [SerializeField] private Sprite[] sprites;
        [SerializeField] private Sprite[] dimSprites;
        [SerializeField] private Sprite[] slotSprites;
        [SerializeField] private Image mainImage;
        [SerializeField] private Image dimImage;
        [SerializeField] private Image slotImage;

        public async override UniTask OnCreate() {
            if (Constants.UsableColors.Contains(Entity.Color)) return;
            mainImage.sprite = sprites[(int)Entity.Color];
            dimImage.sprite = dimSprites[(int)Entity.Color];
            slotImage.sprite = slotSprites[(int)Entity.Color];
            SetForSlot(false);
            SetClickable(false);
        }

        public void SetClickable(bool isClickable) {
            mainImage.enabled = isClickable;
            dimImage.enabled = !isClickable;
        }

        public void SetForSlot(bool isForSlot) {
            mainImage.enabled = !isForSlot;
            dimImage.enabled = !isForSlot;
            slotImage.enabled = isForSlot;
        }

        public void OnClick() {
            TileView.View.OnClickEntity(Entity);
        }

        public async UniTask OnActive() {
            SetForSlot(false);
            SetClickable(true);
        }
    }
}