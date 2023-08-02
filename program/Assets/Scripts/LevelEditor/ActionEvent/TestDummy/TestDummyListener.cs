using UnityEngine;

namespace GemMatch.LevelEditor.ActionEvent {
    public class TestDummyListener : MonoBehaviour {
        public void Awake() {
            EventLinker.ListenForButton(foo, this.gameObject);
        }

        public void OnDestroy() {
            // EventLinker.UnlistenForButton(foo);
        }

        public void foo() {
            Debug.Log("Here!!!");
        }
    }
}