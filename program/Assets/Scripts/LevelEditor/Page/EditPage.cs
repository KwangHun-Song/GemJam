using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using PagePopupSystem;
using Pages;
using UnityEditor;
using UnityEngine;
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
            editInspector.LoadLevel(editInspector.LevelIndex);
#if UNITY_EDITOR
            editInspector.SetDirty();
            Selection.activeObject = editInspector;
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
                GoToPlayPageAsync().Forget();

                async UniTask GoToPlayPageAsync() {
                    SceneManager.LoadScene("Play");
                    await UniTask.WaitUntil(() => SceneManager.GetActiveScene().name.Equals("Play"));
                    await UniTask.DelayFrame(1); // Director에서 MainPage로 이동하는 것 무시
                    PageManager.ChangeImmediately(Page.PlayPage, new PlayPageParam {
                        levelIndex = editInspector.LevelIndex,
                        selectedBoosters = Array.Empty<BoosterIndex>()
                    });
                }
            }
        }
    }
}