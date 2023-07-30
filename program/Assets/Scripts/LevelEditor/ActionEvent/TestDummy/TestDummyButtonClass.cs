using UnityEngine.UI;

namespace GemMatch.LevelEditor.ActionEvent {
    public class TestDummyButtonClass :UnityEngine.MonoBehaviour {
        public Button btn;
        private void Awake() {
            btn = GetComponent<Button>();
            EventLinker.Bind<Button>(btn);
        }

        private void OnDestroy() {
            // EventLinker.Unbind<Button>(btn);
        }
    }
}