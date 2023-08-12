using UnityEngine;

namespace GemMatch {
    public class LevelLoader {
        private static LevelContainer containerCache;
        
        public static LevelContainer GetContainer() {
            return containerCache ??= Resources.Load<LevelContainer>(nameof(LevelContainer));
        }

        public static Level GetLevel(int levelIndex) {
            levelIndex = Mathf.Clamp(levelIndex, 0, GetContainer().levels.Length - 1);
            return GetContainer().levels[levelIndex];
        }
    }
}