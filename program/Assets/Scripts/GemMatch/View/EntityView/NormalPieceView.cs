using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace GemMatch {
    public class NormalPieceView : EntityView {
        [Header("빨, 주, 노, 초, 파, 보 순서로 넣어주세요.")]
        [SerializeField] private Sprite[] sprites;
        [SerializeField] private Image mainImage;

        public async override UniTask OnCreate(Controller controller) {
            Redraw(controller);
        }

        public async override UniTask OnUpdate(Controller controller) {
            Redraw(controller);
        }

        private void Redraw(Controller controller) {
            mainImage.sprite = sprites[(int)Entity.Color];
            mainImage.color = controller.HasPathToTop(controller.GetTile(Entity)) ? Color.white : Color.gray;
        }

        public void OnClick() {
            TileView.View.OnClickEntity(Entity);
        }
    }
}