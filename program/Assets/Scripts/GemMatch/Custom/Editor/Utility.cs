using System;
using System.IO;
using System.Linq;
using UnityEditor;

namespace GemMatch.Custom.Editor {
    public static class Utility {
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
                colorCount = 6
            };

            LevelLoader.GetContainer().levels = new[] { level };
            EditorUtility.SetDirty(LevelLoader.GetContainer());
            AssetDatabase.SaveAssets();

            Tile GetTile(string str) {
                var value = string.IsNullOrEmpty(str) ? 0 : int.Parse(str);
                return new Tile(
                    index:0,
                    isVisible: value > -1,
                    entities: value == -1 ? new [] { new NormalPiece() } : Array.Empty<Entity>()
                );
            }
        }
    }
}