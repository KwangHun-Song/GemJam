using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

namespace GemMatch.Custom.Editor {
    public static class Utility {
        [MenuItem("GemJam/xptmxm")]
        private static void Test() {
            var result = Controller.GenerateColorQueue(36, new List<ColorIndex> {
                ColorIndex.Red, 
                ColorIndex.Orange, 
                ColorIndex.Yellow, 
                ColorIndex.Green, 
                ColorIndex.Blue, 
                ColorIndex.Purple,
                ColorIndex.Brown,
                ColorIndex.Pink,
                ColorIndex.Cyan,
            });
            
            UnityEngine.Debug.Log($"{string.Join(", ", result)}");
        }
        
        [MenuItem("GemJam/임시 기능2 레벨의 모든 색깔 랜덤으로")]
        public static void SetRandomPieceColors() {
            var level = LevelLoader.GetContainer().levels.First();

            foreach (var entityModel in level.tiles.SelectMany(tm => tm.entityModels)) {
                entityModel.color = ColorIndex.Random;
            }
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