using System.Collections.Generic;
using UnityEngine;

namespace GemMatch.LevelEditor {
    public class EditTool : MonoBehaviour {
        [SerializeField] private GameObject selectedPrefab;
        [SerializeField] private Transform toolSpriteMask; // 장치 sprite 위치

        private Camera mainCamera;
        private GameObject selectedObj;
        private IEditToolEventListener _controller;

        private EntityView currentEntityView;
        private readonly List<EntityView> tools = new List<EntityView>();

        public void Initialize(IEditToolEventListener editGameController) {
            this._controller = editGameController;
        }
    }
}