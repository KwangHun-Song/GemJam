using System;
using System.Collections;
using UnityEngine;

namespace GemMatch.LevelEditor {
    public class EditPage : MonoBehaviour {
        [SerializeField] private EditTool editTool;
        [SerializeField] private EditInspector editInspector;
        [SerializeField] private EditGameView editGameView;
        private IEditGameController controller;

        private void OnEnable() {
            // Activate 할 때마다 초기화
            StartCoroutine(CoInitialize());
        }

        private IEnumerator CoInitialize() {
            var ctrl = new EditGameController(editGameView);
            controller = ctrl;
            editTool.Initialize(ctrl);
            editGameView.Initialize(ctrl);
            editInspector.Initialize(ctrl);
            yield return null;
            controller.LoadInspector();
            Draw();
        }

        private void Update() {
            if (Input.GetKeyUp(KeyCode.A)) {
                controller.Input(KeyCode.A);
                PlayTestGame();
            }
        }

        private void Draw() {
            editGameView.Draw();
        }

        private void PlayTestGame() {
            // todo: 바로 PlayScene을 연다
        }
    }
}