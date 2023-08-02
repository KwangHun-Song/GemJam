using System;
using UnityEngine;

namespace GemMatch.LevelEditor {
    public class EditGameController : Controller, IEditGameController {
#region Events
        public event Action<IEditViewEventListener> onTouchForView;
        event Action<IEditViewEventListener> IEditViewEventListener.OnTouch {
            add => onTouchForView += value;
            remove => onTouchForView -= value;
        }

        public event Action<IEditInspectorEventListener> onTouchForInspector;
        event Action<IEditInspectorEventListener> IEditInspectorEventListener.OnTouch {
            add => onTouchForInspector += value;
            remove => onTouchForInspector -= value;
        }

        public event Action<IEditToolEventListener> onTouchForTool;
        event Action<IEditToolEventListener> IEditToolEventListener.OnTouch {
            add => onTouchForTool += value;
            remove => onTouchForTool -= value;
        }
#endregion
        private readonly IEditLinkFromCtrlToView _view;
        private readonly IEditLinkFromCtrlToTool _tool;

        // Memory와 Missions을 사용하지 않는다
        // CurrentLevel,Tiles만 사용
        public EditGameController(IEditLinkFromCtrlToView editGameView) {
            this._view = editGameView;
        }

        public void Initialize(string levelStream) {
            // todo: 레벨 스트림을 그대로 읽어야 할 경우
            ConvertLevel(levelStream);
        }

        private void ConvertLevel(string levelStream) {
            throw new NotImplementedException();
        }

        public event Func<int, Level> OnLoadInspector;
        public void LoadInspector() {
            var currentLevel = OnLoadInspector?.Invoke(1);
            StartGame(currentLevel);
        }

        // EditPage로부터 받는 Input
        public void Input(KeyCode keyCode) {
            switch (keyCode) {
                case KeyCode.A:
                    break;
                case KeyCode.UpArrow:
                case KeyCode.DownArrow:
                case KeyCode.LeftArrow:
                case KeyCode.RightArrow:
                    _view.ResizeBoard(keyCode);
                    break;
            }
        }

        // EditView로부터 받는 Input
        public new void Input(int tileIndex) {
            Tiles[tileIndex] = _tool.GetCurrentTile();
            CurrentLevel.tiles[1] = _tool.GetCurrentTile().Model;
            StartGame(CurrentLevel);
            // todo : tile을 level 또는 SC에 적는다
        }

        public void ChangeTile(Tile tile) {
            CurrentLevel.tiles[tile.Index] = tile.Model.Clone();
        }

        public event Action<Level> OnStartEdit;
        public void StartEdit() {
            var emptyLevel = new Level();
            OnStartEdit?.Invoke(emptyLevel);
        }

        public Level[] GetLevelStatus() {
            // todo: 레벨을 만들기위한 상태추출하기
            return new Level[1]; //
        }

    }
}