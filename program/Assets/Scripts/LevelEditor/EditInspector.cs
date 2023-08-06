using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace GemMatch.LevelEditor {
    public interface IEditCtrlForInspector {
    }

    public class EditInspector : MonoBehaviour, IEditCtrlForInspector {
#region Public Property
        public int LevelIndex {
            get => PlayerPrefs.GetInt("LAST_INDEX", 1);
            set {
                PlayerPrefs.SetInt("LAST_INDEX", value);
                PlayerPrefs.Save();
            }
        }

        public string SavePath { // txt파일로 뽑기 위한 준비
            get {
                return PlayerPrefs.GetString("EDIT_SAVE_PATH", "Assets/Data/text");
            }
            set {
                PlayerPrefs.SetString("EDIT_SAVE_PATH", value);
            }
        }
#endregion
        private IEditInspectorEventListener _contorller;

        public void SetDirty() => EditorUtility.SetDirty(this.gameObject);
        private bool IsDirty() => EditorUtility.IsDirty(this.gameObject);

        public void Initialize(IEditInspectorEventListener gameController) {
            this._contorller = gameController;
        }

        public void LoadLevel(int levelIndex) {
            _contorller.LoadLevel(LevelLoader.GetLevel(levelIndex));
        }

        public void NewLevel() {
            _contorller.MakeLevel1();
        }

        public void ResetLevel() {
            LoadLevel(LevelIndex);
        }

        public void SaveLevel() {
            LevelLoader.GetContainer().levels[LevelIndex] = _contorller.CurrentLevel;
            SetDirty();
            EditorUtility.SetDirty(LevelLoader.GetContainer());
            AssetDatabase.SaveAssets();
        }

        public async UniTask<bool> CheckToRefreshLevel() {
            if (IsDirty()) {
                return true;
            }
            return false;
        }

        public void SetColorCandidates(List<ColorIndex> colorCandidates) {
            _contorller.SetColorCandidates(colorCandidates);
        }
    }
}