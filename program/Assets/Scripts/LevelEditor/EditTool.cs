using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace GemMatch.LevelEditor {
    public interface IEditCtrlForTool {
        Tile GetCurrentTile();
    }

    public partial class EditTool : MonoBehaviour, IEditCtrlForTool {
        [SerializeField] private RectTransform tilePanel;
        [SerializeField] private RectTransform normalEntityPanel;
        [SerializeField] private RectTransform SpawnerEntityPanel;
        [SerializeField] private RectTransform previewRoot;

        private IEditToolEventListener _controller;

        private ToolTileView _currentTileView = null;
        private EditView _view;
        private readonly List<ToolTileView> allTiles = new List<ToolTileView>();

        public void Initialize(IEditToolEventListener editGameController, EditView view) {
            this._controller = editGameController;
            this._view = view;

            // EditTool Tile과 갯수 맞춰서 모델 초기화
            {
                // init emptyTiles
                AddTiles(tilePanel, ModelTemplates.TileToolModel);
                // init normalPieces
                AddTiles(normalEntityPanel, ModelTemplates.NormalToolModels);
                // init spawners // todo: spawner 엔터티를 만들고 테스트
                // AddTiles(SpawnerEntityPanel, initializer.SpawnerToolModels);
            }

            // Inner Method
            void AddTiles(RectTransform root, Tile[] model) {
                var tiles =
                    root.GetComponentsInChildren<ToolTileView>()
                        .Select((t, idx) => {
                            t.Initialize(this, view, model[idx]);
                            return t;
                        });
                allTiles.AddRange(tiles);
            }
        }

        public Tile GetCurrentTile() => _currentTileView?.Tile ?? null;

        public void OnClickToolTile(ToolTileView tileView) {
            if (_currentTileView != null) {
                Destroy(_currentTileView.gameObject);
            }
            this._currentTileView = Instantiate(tileView, previewRoot);
            _currentTileView.Initialize(this, _view, tileView.Tile);
            _currentTileView.name = "Preview";
            _currentTileView.GetComponent<Button>().enabled = false;
            var tr = (_currentTileView.transform as RectTransform);
            tr.anchorMin = new Vector2(0, 0);
            tr.anchorMax = new Vector2(1, 1);
        }
    }
}