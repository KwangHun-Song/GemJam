using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GemMatch.LevelEditor {
    public class EditGameView : UIBehaviour, IEditLinkFromCtrlToView { // view 상속해야할까? 일까?
        [SerializeField] private EditGameBoard board;

        private View _view;//todo 똑같이 할 필요?
        private IEditViewEventListener _controller;
        private EditInspector _inspector;
        private EditTool _editTool;

        private TileView[] tileViews;
        public TileView[] TileViews => tileViews;

        private int height = 11;
        private int width = 9;

        public void Initialize(IEditViewEventListener editCtrl, EditTool editTool, EditInspector editInspector) {
            this._controller = editCtrl;
            this._inspector = editInspector;
            this._editTool = editTool;
            board.Initialize(this, height, width);
        }

        public void SetTileView(List<TileView> tiles) {
            tileViews = tiles.ToArray();
        }

        public void OnClickEntity(Entity entity) {
            var tile = _controller.Tiles.Single(t => t.Entities.Any(e => ReferenceEquals(e, entity)));
            _controller.Input(tile.Index);

            if (tile.Entities.Contains(entity) == false) {
                var tileView = TileViews.Single(tv => tv.Tile == tile);
                var entityView = tileView.RemoveEntityView(entity.Layer);
                entityView.DestroyAsync().Forget();
            }
        }

        public void ResizeBoard(KeyCode keyCode) {
            // todo: 보드 사이즈 줄이기
            switch (keyCode) {
                case KeyCode.UpArrow:
                    if (height > 4) height--;
                    break;
                case KeyCode.DownArrow:
                    if (height < 11) height++;
                    break;
                case KeyCode.LeftArrow:
                    if (width > 4) width--;
                    break;
                case KeyCode.RightArrow:
                    if (width < 9) width++;
                    break;
            }
            board.Initialize(this, height, width);
        }

        public void OnClickTile(TileView tileView) {
            _controller.ChangeTile(tileView.Tile);
        }
    }

    public interface IEditLinkFromCtrlToView {
        void ResizeBoard(KeyCode keyCode);
    }
}