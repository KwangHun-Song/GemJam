using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GemMatch.LevelEditor {
    public class EditGameView : UIBehaviour {
        [SerializeField] private EditGameBoard board;

        private IEditViewEventListener _controller;
        public void Initialize(IEditViewEventListener editGameController) {
            this._controller = editGameController;
        }

        public void Draw() {
            throw new NotImplementedException();
        }
    }
}