using UnityEngine;

namespace GemMatch {
    [CreateAssetMenu(fileName = nameof(LevelContainer))]
    public class LevelContainer : ScriptableObject {
        [SerializeField] public Level[] levels;
    }
}