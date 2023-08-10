using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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
            this.AddComponent<Button>().onClick.AddListener(OnClick);
        }

        public void InjectView(EditView view) {
            this._view = view;
        }

        public void UpdateEditTile(EditView view, Tile tile) {
            this._view = view;
            foreach (var entityView in _tileView.EntityViews.Values) {
                Destroy(entityView.gameObject);
            }

            _tileView.Initialize(view, tile);
            // EntityView의 버튼 인터렉션을 끄고 타일에서 가로챈다
            foreach (EntityView entityView in _tileView.EntityViews.Values) {
                if (entityView.GetComponent<Button>() is Button btn && btn != null) {
                    entityView.GetComponent<Button>().enabled = false;
                }
                if (entityView.Entity.Index == EntityIndex.None) continue;
                entityView.OnCreate().Forget();
                if (entityView is NormalPieceView normal) {
                    normal.SetOnMemoryUI(true);
                } else {
                    // todo: 다른 블럭이 생기면 처리
                }
            }
            this.Tile = this._tileView.Tile;
            this.TileModel = this._tileView.Tile.Model;
            this.gameObject.name = $"Tile({Tile.X},{Tile.Y})";
        }

        // engine은 EntityView만 인터렉션하지만 에디터는 여기서 모두 한다
        public void OnClick() {
            _view.OnClickTileOnBoard(_tileView);
        }
    }
}