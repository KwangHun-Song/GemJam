using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace GemMatch.LevelEditor {
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(TileView))]
    public class ToolTileView : MonoBehaviour, IEditToolView {
        private EditTool _tool;
        private TileView _tileView;

        public Tile Tile { get; private set; }
        public new RectTransform transform => gameObject.transform as RectTransform;
        public TileModel TileModel { get; private set; }
        public EntityModel EntityModel { get; private set; }

        private void OnEnable() {
            this._tileView = this.GetComponent<TileView>();
            var btn = this.GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(OnClick);
        }

        public void Initialize(EditTool tool, EditView view, Tile tile) {
            this._tool = tool;
            _tileView.Initialize(view, tile);
            foreach (EntityView entityView in _tileView.EntityViews.Values) {
                entityView.GetComponent<Button>().enabled = false;
                entityView.GetComponent<Image>().enabled = false;
            }
            var entities = _tileView.EntityViews.Values.Select(t=>t.Entity).ToArray();
            if (entities.Length > 0 && entities[0] != null && entities[0].Model != null)
                EntityModel = entities[0].Model;
            this.Tile = _tileView.Tile;
            this.TileModel = _tileView.Tile.Model;
        }

        // Button Callback
        public void OnClick() {
            _tool.OnClickToolTile(this);
        }
    }
}