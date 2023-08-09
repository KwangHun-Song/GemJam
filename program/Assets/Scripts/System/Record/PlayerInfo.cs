using UnityEngine;

namespace Record {
    public class PlayerInfo {
        public static int HighestClearedLevelIndex {
            get => PlayerPrefs.GetInt(nameof(HighestClearedLevelIndex), -1);
            set => PlayerPrefs.SetInt(nameof(HighestClearedLevelIndex), value);
        }
    }
}