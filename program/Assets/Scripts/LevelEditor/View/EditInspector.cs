using System;
using UnityEditor;
using UnityEngine;

namespace GemMatch.LevelEditor {
    public class EditInspector : MonoBehaviour, IEditGameControllerEventListener {
        public event Action<string> OnFinishLoadLevel;
#region Public Property
        public int LevelIndex {
            get => PlayerPrefs.GetInt("LAST_INDEX", 1);
            set {
                PlayerPrefs.SetInt("LAST_INDEX", value);
                PlayerPrefs.Save();
            }
        }

        public string SavePath {
            get {
                return PlayerPrefs.GetString("EDIT_SAVE_PATH", "Assets/data/text");
            }
            set {
                PlayerPrefs.SetString("EDIT_SAVE_PATH", value);
            }
        }

#endregion

        private void SetDirty() => EditorUtility.SetDirty(this.gameObject);


        private IEditInspectorEventListener _contorller;
        private int width;
        private int height;

        public void Initialize(IEditInspectorEventListener gameController) {
            this._contorller = gameController;
            LoadLevel1();
        }

        private void LoadLevel1() {
            LevelIndex = 0;
            width = 9;
            height = 9;
            LoadLevel(1);
        }

        private void LoadLevel(int levelIndex) {
            var levelExporter = new LevelExporter(SavePath);
            var levelStream = levelExporter.Load(levelIndex);
            OnFinishLoadLevel?.Invoke(levelStream);
        }

        public event Func<IEditGameController> OnSaveLevel;
        public void SaveLevel() {
            // todo: 현재 상태를 파일 형태로 저장
            var fileMaker = new LevelFileMaker(SavePath);
            string levelStream = "레벨 파일 stream";
            fileMaker.SetLevelIndex(LevelIndex);
            fileMaker.SetTargetLevel(_contorller.GetLevelStatus());
            fileMaker.Save();
        }

    }

    public interface IEditGameControllerEventListener {
        event Func<IEditGameController> OnSaveLevel;
    }
}