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
            Redraw();
            SetOnMemoryUI(false);
            SetCanTouchUI(false);
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

        public async UniTask OnActive(bool isActive) {
            SetOnMemoryUI(false);
            SetCanTouchUI(isActive);
        }
    }
}