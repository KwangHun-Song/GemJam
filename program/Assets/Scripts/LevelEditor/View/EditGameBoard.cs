using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GemMatch.LevelEditor {
    [RequireComponent(typeof(GridLayout))]
    public class EditGameBoard : UIBehaviour {
        [SerializeField] private TileView prefabBase;

        private GridLayout gridBoard;
        private int _height = 11;
        private int _width = 9;

        public void Initialize(EditGameView editGameView, int height, int width) {
            this._height = height;
            this._width = width;
            gridBoard = GetComponent<GridLayout>();
            var tiles = new List<TileView>();
            for (int i = 0; i < height * width; i++) {
                var tile = Instantiate(prefabBase, gridBoard.transform);
                var editView = tile.AddComponent<EditEntityView>();
                editView.InjectView(editGameView);
                tiles.Add(tile);
            }

            editGameView.SetTileView(tiles);
        }
    }
}