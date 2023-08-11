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
            NormalPieceView topLayerView = null;
            foreach (EntityView entityView in _tileView.EntityViews.Values) {
                // EntityView의 버튼 인터렉션을 꺼 놓는다
                // EditTileView OnClick으로 대체한다.
                if (entityView.GetComponent<Button>() is Button btn && btn != null) {
                    entityView.GetComponent<Button>().enabled = false;
                }
                if (entityView.Entity.Index == EntityIndex.None) continue;
                entityView.OnCreate().Forget();
                // editor에선 MemorySlot에 들어가는 이미지를 사용한다.
                if (entityView is NormalPieceView normal) {
                    normal.SetOnMemoryUI(true);
                    topLayerView = normal;
                } else {
                    // 다른 블럭 부분
                }
            }
            // 에디터에서 시안성을 위해 NormalPieveView를 가장 앞에 보이도록 놓는다
            if (topLayerView != null)
                topLayerView.transform.SetAsLastSibling();

            this.Tile = this._tileView.Tile;
            this.TileModel = this._tileView.Tile.Model;
            this.gameObject.name = $"Tile({Tile.X},{Tile.Y})";
        }

        // engine은 EntityView만 인터렉션하지만 에디터는 여기서 모두 한다
        public void OnClick() {
            _view.OnClickTileOnBoard(this);
        }
    }
}