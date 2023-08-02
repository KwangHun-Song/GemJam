using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GemMatch.LevelEditor {
    public class EditTool : MonoBehaviour, IEditLinkFromCtrlToTool {
        [SerializeField] private GameObject selectedPrefab;
        [SerializeField] private RectTransform tilePanel;
        [SerializeField] private RectTransform normalEntityPanel;
        [SerializeField] private RectTransform SpawnerEntityPanel;
        [SerializeField] private Transform toolSpriteMask; // 장치 sprite 위치

        private Camera mainCamera;
        private GameObject selectedObj;
        private IEditToolEventListener _controller;

        private EntityView currentEntityView = null;
        private List<EntityView> entities;
        private List<TileView> tiles;

        public void Initialize(IEditToolEventListener editGameController, EditGameView gameView) {
            this._controller = editGameController;

            var ent = normalEntityPanel.GetComponentsInChildren<EntityView>()
                .Concat(SpawnerEntityPanel.GetComponentsInChildren<EntityView>());
            entities = new List<EntityView>(ent);
            tiles = new List<TileView>(tilePanel.GetComponentsInChildren<TileView>());
            foreach (var tileView in tiles) {
                var editTileView = tileView.GetComponent<EditTileView>();
                editTileView.InjectView(gameView);
            }
        }


        public Tile GetCurrentTile() {
            return currentEntityView.TileView.Tile;
        }
    }

    public interface IEditLinkFromCtrlToTool {
        Tile GetCurrentTile();
    }
}