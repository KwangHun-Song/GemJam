using TMPro;
using UnityEngine;

namespace Pages {
    public enum PlayBoosterType {
        Undo, Magnet, Random, Lock, 
    }
    public class PlayBoosterUI {
        [SerializeField] private GameObject goIcon;
        [SerializeField] private GameObject goLock;
        [SerializeField] private TMP_Text txtUnlockLevel;


    }
}