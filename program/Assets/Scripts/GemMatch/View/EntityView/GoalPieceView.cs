using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace GemMatch {
    public class GoalPieceView : EntityView, IReceiveActivation {
        [SerializeField] private Image mainImage;
        
        public void OnClick() {
            TileView.View.OnClickEntity(Entity);
        }

        public async UniTask OnActive() {
            mainImage.color = Color.white;
        }
    }
}