using System.Collections;
using Cysharp.Threading.Tasks;
using PagePopupSystem;
using Pages;
using UnityEditor;
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
                var playpageObjs = FindObjectsOfType<PlayPage>(true);
                foreach (var obj in playpageObjs) {
                    Destroy(obj.gameObject);
                }
                PlayTestGame();
            }

            if (Input.GetKeyUp(KeyCode.Escape)) {
                PageManager.RemovePage(Page.PlayPage);
                var playpageObjs = FindObjectsOfType<PlayPage>(true);
                foreach (var obj in playpageObjs) {
                    Destroy(obj.gameObject);
                }

                playPage = null;
            }
        }

        private void PlayTestGame() {
            if (FindObjectOfType<EditLevelIndicator>() == null) {
                var indicator = new GameObject();
                indicator.AddComponent<EditLevelIndicator>();
                DontDestroyOnLoad(indicator);
            }
            LoadPlayPageAsync().Forget();
        }

        private GameObject playPage = null;
        private async UniTask LoadPlayPageAsync() {
            var obj = await Resources.LoadAsync<GameObject>("Editor_PlayPage");
            playPage = Instantiate((GameObject)obj, null);
            playPage.name = "PlayPage";
            await UniTask.DelayFrame(2);
            var script = playPage.GetComponent<PlayPage>();
            script.StartGame(editInspector.LevelIndex);
        }
    }
}