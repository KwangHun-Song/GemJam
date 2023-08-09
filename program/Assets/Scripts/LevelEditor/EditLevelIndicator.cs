using UnityEngine;

namespace GemMatch.LevelEditor {
    public class EditLevelIndicator : MonoBehaviour {
        public int LevelIndex => PlayerPrefs.GetInt(Constants.LevelIndexPrefsKey, 0);
    }
}