using System;
using UnityEditor;
using UnityEngine;

namespace GemMatch.LevelEditor {
    public class EditInspector : MonoBehaviour {
        private IEditGameController _editGameController = null;

        public event Action<IEditGameController> OnLoadLevel;

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


        public void Initialize(IEditGameController gameController) {
            _editGameController = gameController;
            LoadLevel1();
        }

        private void LoadLevel1() {
            if (_editGameController == null) return;

            LoadLevel(1);
        }

        private void LoadLevel(int levelIndex) {
            // todo: 레벨파일을 읽어 gameController 갱신
            var levelExporter = new LevelExporter(SavePath);
            var levelStream = levelExporter.Load(levelIndex);
            _editGameController.Initialize(levelStream);
            OnLevelLoad?.Invoke(_editGameController);
        }

        public void SaveLevel() {
            // todo: 현재 상태를 파일 형태로 저장
            var fileMaker = new LevelFileMaker(SavePath);
            string levelStream = "레벨 파일 stream";
            fileMaker.SetLevelIndex(LevelIndex);
            fileMaker.SetTargetLevel(_editGameController);
            fileMaker.Save();
        }

    }
}