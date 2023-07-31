using System;
using System.Collections;
using UnityEngine;

namespace GemMatch.LevelEditor {
    public class EditPage : MonoBehaviour {
        [SerializeField] private EditTool editTool;
        [SerializeField] private EditInspector editInspector;
        [SerializeField] private EditGameView editGameView;
        private IEditGameController editGameController;

        private void Start() {
            StartCoroutine(CoInitialize());
        }

        private IEnumerator CoInitialize() {
            // todo: edit view component 초기화
            editGameController = new EditGameController(editGameView);
            editTool.Initialize();
            editGameView.Initialize();
            editInspector.Initialize(editGameController);
            yield return null;
            LoadInspector();
            Draw();
        }

        private void LoadInspector() {

        }

        private void Draw() {
            throw new NotImplementedException();
        }

        public void PlayTestGame() {
            editGameController.Play();
        }
    }
}