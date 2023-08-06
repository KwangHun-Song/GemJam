using UnityEngine;
using UnityEngine.UI;

namespace GemMatch.LevelEditor {
    [RequireComponent(typeof(TileView))]
    [RequireComponent(typeof(Button))]
    public class ToolTileView : MonoBehaviour{
        private EditTool _tool;
        private TileView _tileView;

        public Tile Tile { get; private set; }
        public TileModel TileModel { get; private set; }

        private void OnEnable() {
            this._tileView = this.GetComponent<TileView>();
            this.GetComponent<Button>().onClick = null;
            this.GetComponent<Button>().onClick.AddListener(OnClick);
        }

        public void Initialize(EditTool tool, EditView view, Tile tile) {
            this._tool = tool;
            _tileView.Initialize(view, tile);
            this.Tile = this._tileView.Tile;
            this.TileModel = this._tileView.Tile.Model;
        }

        // Button Callback
        public void OnClick() {
            _tool.OnClickToolTile(this);
        }
    }
}