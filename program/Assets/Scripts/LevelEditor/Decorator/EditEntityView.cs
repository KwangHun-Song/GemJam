using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace GemMatch.LevelEditor {
    /// <summary>
    /// EditTool에 있어 모델이 된다. EntityView의 wrapper class
    /// </summary>
    [RequireComponent(typeof(EntityView))]
    public class EditEntityView : MonoBehaviour { //todo: 안쓰면 지우기
        private EntityView _entityView;
        private EditView _view;
        public Entity Entity { get; private set; }

        private void OnEnable() {
            this._entityView = this.GetComponent<EntityView>();
            this._entityView.GetComponent<Button>().onClick.AddListener(OnClick);
        }

        public void InjectView(EditView view) {
            this._view = view;
        }

        public async UniTask OnCreate() {
            await _entityView.OnCreate();
        }

        public void OnClick() {
            Assert.IsNotNull(_view);
            // _view.OnClickEditEntity(_entityView.Entity);
        }
    }
}