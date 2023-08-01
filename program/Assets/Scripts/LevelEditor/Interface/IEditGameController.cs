using System;
using UnityEngine;

namespace GemMatch.LevelEditor {
    public interface IEditGameController {
        void Initialize(string levelStream); // level stream으로 데이터를 채운다
        void LoadInspector();
        void Input(KeyCode keyCode);
        void Play();
    }

    public interface IEditToolEventListener {}
    public interface IInspectorEventListener {
        LevelSchema GetLevelStatus();
    }
    public interface IEditViewEventListener {}

    public class EditGameController : IEditGameController, IEditToolEventListener, IEditViewEventListener, IInspectorEventListener {
        public event Action OnSelectAll;

        public EditGameController(EditGameView editGameView) {
            throw new System.NotImplementedException();
        }

        public void Initialize(string levelStream) {

            throw new System.NotImplementedException();
        }


        public void Play() {
            throw new System.NotImplementedException();
        }

        public event Action<int> OnLoadInspector;
        public void LoadInspector() {
            OnLoadInspector?.Invoke(1);
        }

        public void Input(KeyCode keyCode) {
            switch (keyCode) {
                case KeyCode.A:
                    OnSelectAll?.Invoke();
                    break;
            }
        }

        public LevelSchema GetLevelStatus() {
            // todo: 레벨을 만들기위한 상태추출하기
            return new LevelSchema(); //
        }
    }

    public class LevelSchema {
       public Level[] levels;
    } // todo: LevelContainer로 바뀌어질 클래스
}