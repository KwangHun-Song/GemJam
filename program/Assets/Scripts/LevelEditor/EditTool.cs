using UnityEngine;

namespace GemMatch.LevelEditor {
    public class EditTool : MonoBehaviour {
        [SerializeField] private GameObject selectedPrefab;
        [SerializeField] private Transform toolSpriteMask; // 장치 sprite 위치

        private Camera mainCamera;
        private GameObject selectedObj;

        private EntityView currentEntityView;
        // private readonly List<EntityView> tools = new List<EntityView>();
        // public

        public void Initialize() {
            throw new System.NotImplementedException();
        }
    }
}