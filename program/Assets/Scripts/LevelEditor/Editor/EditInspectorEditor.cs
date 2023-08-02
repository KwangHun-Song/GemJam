using System;
using UnityEditor;

namespace GemMatch.LevelEditor {
    [CustomEditor((typeof(EditInspector)))]
    public class EditInspectorEditor : Editor {

        private void Awake() {
            var t = target as EditInspector;
            if (t == null) {
                throw new NullReferenceException("NEED EditorInspector");
            }
            // t.OnLoadLevel += OnLoadLevel;
        }

        private void OnDisable() {
            isLevelLoaded = false;
        }

        private bool isLevelLoaded = false;
        private void OnLoadLevel(IEditGameController controller) {
            // todo : EditInspector가 controller갱신 시 notify
            isLevelLoaded = true;
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            var t = target as EditInspector;
            if (t == null || false == isLevelLoaded) return;
        }
    }
}