using System.Collections;
using PagePopupSystem;
using Pages;
using Record;
using UnityEditor;
using UnityEngine;

namespace GemMatch.LevelEditor {
    public class EditPage : PageHandler {
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
                PlayerInfo.HighestClearedLevelIndex = editInspector.LevelIndex;
                ChangeTo(Page.PlayPage, new PlayPageParam {
                    levelIndex = editInspector.LevelIndex,
                    selectedBoosters = new BoosterIndex[0]{}
                });
            }
        }

        public override SoundName BgmName => SoundName.None;
        public override Page GetPageType() => Page.EditPage;
    }
}