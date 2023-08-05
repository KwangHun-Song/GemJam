using System.Collections.Generic;
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

        private const int Height = 11;
        private const int Width = 8;

        private GridLayoutGroup gridBoard;
        private readonly List<EditTileView> tilesOnBoard = new List<EditTileView>();

        private void OnEnable() {
            gridBoard = GetComponent<GridLayoutGroup>();
        }

        public void Initialize(EditView editView) {
            tilesOnBoard.Clear();
            for (int i = 0; i < Height * Width; i++) {
                var tile = Instantiate(prefabBase, gridBoard.transform);
                tile.name = $"Tile({i % Width},{i / Width})";
                var editTileView = tile.AddComponent<EditTileView>();
                editTileView.InjectView(editView);
                tilesOnBoard.Add(editTileView);
            }

            editView.SetTileView(tilesOnBoard.ToArray());
        }
    }
}