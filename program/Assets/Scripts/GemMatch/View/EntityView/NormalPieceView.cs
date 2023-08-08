using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace GemMatch {
    public class NormalPieceView : EntityView, IReceiveActivation {
        [Header("빨, 주, 노, 초, 파, 보 순서로 넣어주세요.")]
        [SerializeField] private Sprite[] sprites;
        [SerializeField] private Image mainImage;

        public async override UniTask OnCreate() {
            if (Entity.Color > ColorIndex.Cyan || Entity.Color < 0) return;
            mainImage.sprite = sprites[(int)Entity.Color];
            mainImage.color = Color.gray;
        }

        public void OnClick() {
            TileView.View.OnClickEntity(Entity);
        }

        public async UniTask OnActive() {
            mainImage.color = Color.white;
        }
    }
}