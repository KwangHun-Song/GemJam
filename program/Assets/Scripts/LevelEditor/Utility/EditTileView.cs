using UnityEngine;

namespace GemMatch.LevelEditor {
    /// <summary>
    /// EditTool에 있어 모델이 된다.
    /// </summary>
    [RequireComponent(typeof(TileView))]
    public class EditTileView : MonoBehaviour {
        private TileView _tileView;
        private EditGameView _view;

        private void OnEnable() {
            this._tileView = this.GetComponent<TileView>();
        }

        public void InjectView(EditGameView view) {
            this._view = view;
        }

        public void OnClick() {
            // todo : 모델 타일 클릭 했을때 처리
            _view.OnClickTile(_tileView);
        }
    }
}