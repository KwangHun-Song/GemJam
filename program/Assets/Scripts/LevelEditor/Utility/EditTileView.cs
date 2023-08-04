using System;
using System.Collections.Generic;
using UnityEngine;

namespace GemMatch.LevelEditor {
    /// <summary>
    /// EditTool에 있어 모델이 된다. TileView의 wrapper class
    /// </summary>
    [RequireComponent(typeof(TileView))]
    public class EditTileView : MonoBehaviour {
        private TileView _tileView;
        private EditGameView _view;

        public Tile Tile { get; private set; }
        public List<EntityView> EntityViews { get; private set; }

#if UNITY_EDITOR
        private void OnGUI() {
            if (_tileView?.Tile == null) return;
            GUILayout.Label($"({_tileView.Tile.Index})");
        }
#endif

        private void OnEnable() {
            this._tileView = this.GetComponent<TileView>();
            this.Tile = this._tileView.Tile;
            this.EntityViews = this._tileView.EntityViews;
        }

        public void InjectView(EditGameView view) {
            this._view = view;
        }

        public void Initialize(EditGameView view, Tile tile) {
            _view = view;
            var tmpView = _tileView.View;
            _tileView.Initialize(tmpView, tile);
        }

        public void OnClick() {
            // todo : 모델 타일 클릭 했을때 처리
            _view.OnClickTile(_tileView);
        }

        public EntityView RemoveEntityView(Layer layer) {
            return _tileView.RemoveEntityView(layer);
        }
    }
}