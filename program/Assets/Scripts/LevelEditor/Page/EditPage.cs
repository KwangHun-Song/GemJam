using System;
using System.Collections;
using UnityEngine;

namespace GemMatch.LevelEditor {
    public class EditPage : MonoBehaviour {
        [SerializeField] private EditTool editTool;
        [SerializeField] private EditInspector editInspector;
        [SerializeField] private EditGameView editGameView;
        private EditGameController controller;

        private void OnEnable() {
            // Activate 할 때마다 초기화
            StartCoroutine(CoInitialize());
        }

        private IEnumerator CoInitialize() {
            var ctrl = new Controller();
            var editCtrl = new EditGameController(editGameView);
            controller = editCtrl;
            editGameView.Initialize(editCtrl as IEditViewEventListener, editTool, editInspector);
            editTool.Initialize(editCtrl, editGameView);
            editInspector.Initialize(editCtrl);
            yield return null;
            controller.LoadInspector();
        }

        public bool IsLoaded { get; set; }
        private void Update() {
            if (IsLoaded == false) return;
            InputMouse();
            InputKeyboard();
        }

        private void InputMouse() {
            if (Input.GetMouseButtonUp(0)) {
                controller.Input(KeyCode.Mouse0);
            } else if (Input.GetMouseButtonUp(1)) {
                controller.Input(KeyCode.Mouse1);
            } else if (Input.GetMouseButtonUp(3)) {
                controller.Input(KeyCode.Mouse2);
            }
        }

        private void InputKeyboard() {
            if (Input.GetKeyUp(KeyCode.A)) {
                controller.Input(KeyCode.A);
                PlayTestGame();
            }
        }

        private void PlayTestGame() {
            // todo: 바로 PlayScene을 연다
        }
    }
}