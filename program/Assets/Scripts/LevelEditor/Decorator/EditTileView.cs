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
            this.Tile = _tileView.Tile;
            this.TileModel = _tileView.Tile.Model;
        }

        public void InjectView(EditView view) {
            this._view = view;
        }

        public void UpdateEditTile(EditView view, Tile tile) {
            this._view = view;
            _tileView.Initialize(view, tile);
            this.Tile = this._tileView.Tile;
            this.TileModel = this._tileView.Tile.Model;
        }

        // engine은 EntityView만 인터렉션하지만 에디터는 여기서 모두 한다
        public void OnClick() {
            _view.OnClickTileOnBoard(_tileView);
        }

        public EntityView RemoveEntityView(Layer layer) {
            return _tileView.RemoveEntityView(layer);
        }
    }
}