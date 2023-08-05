using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

namespace GemMatch.LevelEditor {
    public interface IEditCtrlEventToView {
        void UpdateBoard(List<Tile> tiles);
        void UpdateBoard(Tile tile);
    }

    public class EditView : View, IEditCtrlEventToView { // view 상속해야할까? 일까?
        [SerializeField] private EditTileBoard board;

        private IEditViewEventListener _controller;
        private EditInspector _inspector;
        private EditTool _editTool;

        public EditTileView[] TileViews { get; private set; }

        public void Initialize(IEditViewEventListener editCtrl, EditTool editTool, EditInspector editInspector) {
            this._controller = editCtrl;
            this._inspector = editInspector;
            this._editTool = editTool;
            board.Initialize(this);
        }

        public void SetTileView(EditTileView[] tiles) {
            TileViews = tiles;
        }


        public void OnClickEditEntity(Entity entityViewEntity) {
            _controller.ChangeTile();
        }

        public void OnClickEntity(Entity entity) {
            var tile = _controller.Tiles.Single(t => t.Entities.Any(e => ReferenceEquals(e, entity)));
            _controller.Input(tile.Index);

            if (tile.Entities.Contains(entity) == false) {
                EditTileView tileView = TileViews.Single(tv => tv.Tile == tile);
                var entityView = tileView.RemoveEntityView(entity.Layer);
                entityView.DestroyAsync().Forget();
            }
        }

        void IEditCtrlEventToView.UpdateBoard(List<Tile> tiles) {
            for (int i = 0; i < tiles.Count; i++) {
                TileViews[i].Initialize(this, tiles[i]);
            }
        }

        public void UpdateBoard(Tile tile) {
            var targetTile = TileViews.FirstOrDefault(t => t.Tile.Index == tile.Index);
            Assert.IsNotNull(targetTile);
            targetTile.Initialize(this, tile);
        }

        public void OnClickTile(TileView tileView) {
            _controller.ChangeTile(tileView.Tile.Clone());
        }
    }
}