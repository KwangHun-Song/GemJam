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
            if (Entity.Color != ColorIndex.Random)
                if (Constants.UsableColors.Contains(Entity.Color) == false) return;
            var index = Entity.Color == ColorIndex.Random ? sprites.Length-1 : (int)Entity.Color;
            mainImage.sprite = sprites[index];
            dimImage.sprite = dimSprites[index];
            slotImage.sprite = slotSprites[index];
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