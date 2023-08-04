using Cysharp.Threading.Tasks;
using UnityEngine;

namespace GemMatch.LevelEditor {
    /// <summary>
    /// EditTool에 있어 모델이 된다. EntityView의 wrapper class
    /// </summary>
    [RequireComponent(typeof(EntityView))]
    public class EditEntityView : MonoBehaviour {
        private EntityView _entityView;
        private EditGameView _view;

        private void OnEnable() {
            this._entityView = this.GetComponent<EntityView>();
        }

        public void InjectView(EditGameView view) {
            this._view = view;
        }

        public async UniTask OnCreate() {
            await _entityView.OnCreate();
        }

        public void OnClick() {
            _view.OnClickEntity(_entityView.Entity);
        }
    }
}