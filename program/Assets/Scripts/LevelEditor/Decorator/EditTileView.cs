using System.Collections.Generic;
using UnityEngine;

namespace GemMatch.LevelEditor {
    /// <summary>
    /// EditTool에 있어 모델이 된다. TileView의 wrapper class
    /// </summary>
    [RequireComponent(typeof(TileView))]
    public class EditTileView : MonoBehaviour {
        private TileView _tileView;
        private EditView _view;

        public Tile Tile { get; private set; }
        public TileModel TileModel { get; private set; }

#if UNITY_EDITOR
        private void OnGUI() {
            if (_tileView?.Tile == null) return;
            GUILayout.Label($"({_tileView.Tile.Index})");
        }
#endif

        private void OnEnable() {
            this._tileView = this.GetComponent<TileView>();
        }

        public void InjectView(EditView view) {
            this._view = view;
        }

        public void Initialize(EditView view, Tile tile) {
            this._view = view;
            _tileView.Initialize(view, tile);
            this.Tile = this._tileView.Tile;
            this.TileModel = this._tileView.Tile.Model;
        }

        public void OnClick() {
            // todo : 모델 타일 클릭 했을때 처리
            _view.OnClickTileOnToolbox(_tileView);
        }

        public EntityView RemoveEntityView(Layer layer) {
            return _tileView.RemoveEntityView(layer);
        }
    }
}