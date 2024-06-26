using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

namespace GemMatch.Custom.Editor {
    public static class MenuItemsImpl {
        [MenuItem("GemJam/임시 기능2 레벨의 모든 색깔 랜덤으로")]
        public static void SetRandomPieceColors() {
            var level = LevelLoader.GetContainer().levels.First();

            foreach (var tileModel in level.tiles.Where(t => t.entityModels.Any(em => em.layer == Layer.Cover))) {
                tileModel.entityModels.Single(em => em.layer == Layer.Cover).index = EntityIndex.InvisibleCover;
            }

            // foreach (var entityModel in level.tiles.SelectMany(tm => tm.entityModels)) {
            //     entityModel.color = ColorIndex.Random;
            // }
        }
        
        [MenuItem("GemJam/임시 기능")]
        public static void TempMenuItem() {
            var lines = File.ReadAllLines("Assets/Data/text/dummy.csv").Skip(1).Reverse();
            var rows = lines
                .Select(l => l.Split(',').Skip(1).Select(GetTile))
                .SelectMany(a => a)
                .ToArray();

            for (int i = 0; i < rows.Length; i++) {
                rows[i].index = i;
            }

            var level = new Level {
                tiles = rows,
                colorCount = 6,
                missions = rows
                    .SelectMany(t => t.entityModels)
                    .Where(em => em.color >= 0)
                    .GroupBy(em => em.color)
                    .Select(g => new Mission {
                        entity = new EntityModel { index = EntityIndex.NormalPiece, layer = Layer.Piece, color = g.Key },
                        count = g.Count()
                }).ToArray()
            };

            LevelLoader.GetContainer().levels = new[] { level };
            EditorUtility.SetDirty(LevelLoader.GetContainer());
            AssetDatabase.SaveAssets();

            TileModel GetTile(string str) {
                var value = string.IsNullOrEmpty(str) ? 0 : int.Parse(str);
                return new TileModel {
                    isOpened = value >= -1,
                    entityModels = value >= 0 ? new List<EntityModel> { new EntityModel {
                        index = EntityIndex.NormalPiece,
                        layer = Layer.Piece,
                        color = (ColorIndex)value,
                    }} : new List<EntityModel>()
                };
            }
        }
    }
}