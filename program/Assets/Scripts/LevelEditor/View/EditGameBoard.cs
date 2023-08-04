using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GemMatch.LevelEditor {
    [RequireComponent(typeof(GridLayoutGroup))]
    public class EditGameBoard : UIBehaviour {
        [SerializeField] private TileView prefabBase;

        private const int Height = 11;
        private const int Width = 8;

        private GridLayoutGroup gridBoard;
        private readonly List<EditTileView> editViews = new List<EditTileView>();

        private void OnEnable() {
            gridBoard = GetComponent<GridLayoutGroup>();
        }

        public void Initialize(EditGameView editGameView) {
            editViews.Clear();
            for (int i = 0; i < Height * Width; i++) {
                var tile = Instantiate(prefabBase, gridBoard.transform);
                tile.name = $"Tile({i % Width},{i / Width})";
                var editView = tile.AddComponent<EditTileView>();
                editView.InjectView(editGameView);
                editViews.Add(editView);
            }

            editGameView.SetTileView(editViews);
        }

        public void Resize(int height, int width) {
            // todo : tile model을 좌우, 위아래 번갈아가면서 isOpen을 닫는 알고리즘
        }
    }
}