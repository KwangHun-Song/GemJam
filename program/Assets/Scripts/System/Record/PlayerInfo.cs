using UnityEngine;

namespace Record {
    public class PlayerInfo {
        public static bool EnableSfx {
            get => PlayerPrefs.GetInt(nameof(EnableSfx), 1) == 1;
            set => PlayerPrefs.SetInt(nameof(EnableSfx), value ? 1 : 0);
        }
        
        public static bool EnableBgm {
            get => PlayerPrefs.GetInt(nameof(EnableBgm), 1) == 1;
            set => PlayerPrefs.SetInt(nameof(EnableBgm), value ? 1 : 0);
        }
        
        public static int HighestClearedLevelIndex {
            get => PlayerPrefs.GetInt(nameof(HighestClearedLevelIndex), 0);
            set => PlayerPrefs.SetInt(nameof(HighestClearedLevelIndex), value);
        }
    }
}