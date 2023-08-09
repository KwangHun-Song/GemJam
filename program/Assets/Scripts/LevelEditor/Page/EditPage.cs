using System.Collections;
using Cysharp.Threading.Tasks;
using Pages;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace GemMatch.LevelEditor {
    public class EditPage : MonoBehaviour {
        [SerializeField] private EditTool editTool;
        [SerializeField] private EditInspector editInspector;
        [SerializeField] private EditView editView;

        private EditController controller;

        private void OnEnable() {
            // Activate 할 때마다 초기화
            StartCoroutine(CoInitialize());
        }

        private IEnumerator CoInitialize() {
            yield return null;
            yield return null;
            var editCtrl = new EditController(editView, editTool);
            controller = editCtrl;
            editView.Initialize(editCtrl);
            editTool.Initialize(editCtrl, editView);
            editInspector.Initialize(editCtrl);
            yield return null;
            editInspector.LoadLevel(0);
#if UNITY_EDITOR
            editInspector.SetDirty();
#endif
            IsLoaded = true;
        }

        public bool IsLoaded { get; set; }

        private void Update() {
            if (IsLoaded == false) return;
            InputKeyboard();
        }

        private void InputKeyboard() {
            if (Input.GetKeyUp(KeyCode.A)) {
                PlayTestGame();
            }

            // KeyCode[] usableKeys = new[] {
            //     KeyCode.UpArrow,
            //     KeyCode.DownArrow,
            //     KeyCode.LeftArrow,
            //     KeyCode.RightArrow,
            // };
            // foreach (var key in usableKeys) {
            //     if (Input.GetKeyUp(key)) {
            //         controller.Input(key);
            //     }
            // }
        }

        private void PlayTestGame() {
            if (FindObjectOfType<EditLevelIndicator>() == null) {
                var indicator = new GameObject();
                indicator.AddComponent<EditLevelIndicator>();
                DontDestroyOnLoad(indicator);
            }
            LoadPlayPageAsync().Forget();
        }

        private async UniTask LoadPlayPageAsync() {
            await SceneManager.LoadSceneAsync("Play");
            await UniTask.DelayFrame(2);
            var playpage = FindObjectOfType<PlayPage>();
            Assert.IsNotNull(playpage);
            playpage.StartGame();
        }
    }
}