using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GemMatch.LevelEditor {
    public interface IEditCtrlForTool {
        Tile GetCurrentTile();
    }

    public class EditTool : MonoBehaviour, IEditCtrlForTool {
        [SerializeField] private RectTransform tilePanel;
        [SerializeField] private RectTransform normalEntityPanel;
        [SerializeField] private RectTransform SpawnerEntityPanel;
        [SerializeField] private GameObject previewBase;
        [SerializeField] private RectTransform previewRoot;

        private IEditToolEventListener _controller;

        private ToolTileView _currentTileView = null;
        private List<ToolTileView> allTiles;

        public void Initialize(IEditToolEventListener editGameController, EditView view) {
            this._controller = editGameController;

            // EditTool Tile과 갯수 맞춰서 모델 초기화
            var initializer = new ToolInitializer();
            {
                // init emptyTiles
                AddTiles(tilePanel, initializer.TileToolModel);
                // init normalPieces
                AddTiles(normalEntityPanel, initializer.NormalToolModels);
                // init spawners
                AddTiles(SpawnerEntityPanel, initializer.SpawnerToolModels);
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

        public bool IsEntityFocused => _currentTileView.TileModel.entityModels.Count > 0;
        public bool IsEmptyTileFocused => _currentTileView.TileModel.entityModels.Count == 0;

        public Tile GetCurrentTile() => _currentTileView.Tile;

        public void OnClickToolTile(ToolTileView tileView) {
            this._currentTileView = tileView;
            var parent = previewRoot.parent;
            Destroy(previewRoot);
            previewRoot = Instantiate(previewBase, parent).GetComponent<RectTransform>();
        }

        private class ToolInitializer {
            public Tile[] TileToolModel {
                get {
                    var isOpeneds = new []{true,false};
                    return isOpeneds.Select((t, idx) => {
                        return new Tile(new TileModel() {
                            entityModels = new List<EntityModel>(),
                            index = 0,
                            isOpened = isOpeneds[idx] }); }).ToArray();
                }
            }

            private ColorIndex[] Colors = new[] {
                ColorIndex.Red,
                ColorIndex.Orange,
                ColorIndex.Yellow,
                ColorIndex.Green,
                ColorIndex.Blue,
                ColorIndex.Purple,
                ColorIndex.Random
            };
            public Tile[] NormalToolModels {
                get {
                    var tiles = new List<Tile>();
                    foreach (var color in Colors)
                        tiles.Add(new Tile(
                            new TileModel() {
                                entityModels = new List<EntityModel>() {
                                    new EntityModel() {
                                        color = color,
                                        displayType = 0,
                                        durability = 0,
                                        index = EntityIndex.NormalPiece,
                                        layer = Layer.Piece
                                    }
                                },
                                index = 0,
                                isOpened = true
                            }
                        ));
                    return tiles.ToArray();
                }
            }

            private ColorIndex[] SpawnTypes = new[] { //todo: spawner가 생기면 수정
                ColorIndex.Red,
                ColorIndex.Orange,
                ColorIndex.Yellow,
            };
            public Tile[] SpawnerToolModels {
                get {
                    var tiles = new List<Tile>();
                    foreach (var spawnType in SpawnTypes)
                        tiles.Add(new Tile(
                            new TileModel() {
                                entityModels = new List<EntityModel>() {
                                    new EntityModel() {
                                        color = spawnType,
                                        displayType = 0,
                                        durability = 0,
                                        index = EntityIndex.SpawnerPiece,
                                        layer = Layer.Piece
                                    }
                                },
                                index = 0,
                                isOpened = true
                            }
                        ));
                    return tiles.ToArray();
                }
            }
        }
    }
}