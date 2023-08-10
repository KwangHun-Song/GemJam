using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace GemMatch.LevelEditor {
    public interface IEditCtrlForView {
        void UpdateBoard(List<Tile> tiles);
        void UpdateBoard(Tile tile);
    }

    public class EditView : View, IEditCtrlForView {
        [SerializeField] private EditTileBoard board;

        private IEditViewEventListener _controller;

        public EditTileView[] EditTileViews { get; private set; }

        public void Initialize(IEditViewEventListener editCtrl) {
            this._controller = editCtrl;
            board.Initialize(this);
        }

        public void SetTileView(EditTileView[] tiles) {
            EditTileViews = tiles;
        }

        void IEditCtrlForView.UpdateBoard(List<Tile> tiles) {
            for (int i = 0; i < tiles.Count; i++) {
                EditTileViews[i].UpdateEditTile(this, tiles[i]);
            }
        }

        void IEditCtrlForView.UpdateBoard(Tile tile) {
            var targetTile = EditTileViews.Single(t => t.Tile == tile);
            Assert.IsNotNull(targetTile);
            targetTile.UpdateEditTile(this, tile);
        }

        public void OnClickTileOnBoard(TileView tileView) {
            var tile = _controller.ChangeTile(tileView.Tile.Model);
            board.UpdateTileView(tile);
        }

        internal override EntityView CreateEntityView(Entity entity) {
            var prefab = Resources.Load<EntityView>($"Editor_{entity.Index}");
            var view = Instantiate(prefab, null, true);
            view.Initialize(null, entity);

            return view;
        }
    }
}