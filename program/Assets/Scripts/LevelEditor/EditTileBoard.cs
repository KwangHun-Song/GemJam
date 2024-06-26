using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GemMatch.LevelEditor {
    /// <summary>
    /// EditBoard에 TileView를 View 대신 생성
    /// </summary>
    [RequireComponent(typeof(GridLayoutGroup))]
    public class EditTileBoard : UIBehaviour {
        [SerializeField] private TileView prefabBase;

        private int Height = Constants.Height;
        private int Width = Constants.Width;

        private GridLayoutGroup gridBoard;
        private EditView _view;
        private readonly List<EditTileView> tilesOnBoard = new List<EditTileView>();

        protected override void OnEnable() {
            gridBoard = GetComponent<GridLayoutGroup>();
        }

        public void Initialize(EditView editView) {
            this._view = editView;
            tilesOnBoard.Clear();
            for (int i = 0; i < Height * Width; i++) {
                var tile = Instantiate(prefabBase, gridBoard.transform);
                var editTileView = tile.AddComponent<EditTileView>();
                editTileView.InjectView(editView);
                tilesOnBoard.Add(editTileView);
            }

            editView.SetTileView(tilesOnBoard.ToArray());
        }

        public void UpdateTileView(Tile tile) {
            var targetView = tilesOnBoard.Single(t => t.TileModel.index == tile.Index);
            targetView.UpdateEditTile(_view, tile);
        }

        private void OnDisable() {
            foreach (EditTileView view in tilesOnBoard) {
                Destroy(view.gameObject);
            }
            tilesOnBoard.Clear();
        }
    }
}