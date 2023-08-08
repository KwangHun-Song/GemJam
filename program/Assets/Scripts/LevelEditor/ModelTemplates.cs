using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace GemMatch.LevelEditor {

    public static class ModelTemplates {
        public static ColorIndex[] AllColors = new[] {
            ColorIndex.Red,
            ColorIndex.Orange,
            ColorIndex.Yellow,
            ColorIndex.Green,
            ColorIndex.Blue,
            ColorIndex.Purple,
            ColorIndex.Random
        };

        public static EntityModel NormalEntityModel(ColorIndex color) => new EntityModel() {
            index = EntityIndex.NormalPiece,
            layer = Layer.Piece,
            displayType = 0,
            durability = 0,
            color = color,
            commonStringParam = string.Empty,
        };

        public static Tile[] TileToolModel {
            get {
                var isOpeneds = new []{true,false};
                return isOpeneds.Select((t, idx) => {
                    return new Tile(new TileModel() {
                        index = 0,
                        isOpened = isOpeneds[idx],
                    });
                }).ToArray();
            }
        }



        public static Tile[] NormalToolModels {
            get {
                var tiles = new List<Tile>();
                foreach (var color in AllColors)
                    tiles.Add(new Tile(
                        new TileModel() {
                            entityModels = new List<EntityModel>() {
                                NormalEntityModel(color)
                            },
                            index = 0,
                            isOpened = true
                        }
                    ));
                return tiles.ToArray();
            }
        }

        private static ColorIndex[] SpawnTypes = new[] { //todo: spawner가 생기면 수정
            ColorIndex.Red,
            ColorIndex.Orange,
            ColorIndex.Yellow,
        };
        public static Tile[] SpawnerToolModels {
            get {
                var tiles = new List<Tile>();
                foreach (var spawnType in SpawnTypes)
                    tiles.Add(new Tile(
                        new TileModel() {
                            entityModels = new List<EntityModel>() {
                                new EntityModel() {
                                    index = EntityIndex.SpawnerPiece,
                                    layer = Layer.Piece,
                                    displayType = 0,
                                    durability = 0,
                                    color = spawnType,
                                    commonStringParam = string.Empty,
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