using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace GemMatch.LevelEditor {
    public class EditInspector : MonoBehaviour {
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

        private IEditInspectorEventListener _contorller;
#endregion

        public void SetDirty() => EditorUtility.SetDirty(this.gameObject);
        private bool IsDirty() => EditorUtility.IsDirty(this.gameObject);


        private int width;
        private int height;

        public void Initialize(IEditInspectorEventListener gameController) {
            this._contorller = gameController;
            StartLevel1();
        }

        public void StartLevel1() {
            _contorller.MakeLevel1();
        }

        public void LoadLevel(int levelIndex) {
            _contorller.LoadLevel(LevelLoader.GetLevel(levelIndex));
        }

        public event Func<List<Level>, List<Level>> OnSaveLevel;
        public void SaveLevel() {
            var lvs = OnSaveLevel?.Invoke(LevelLoader.GetContainer().levels.ToList());
            LevelLoader.GetContainer().levels = lvs.ToArray();
            EditorUtility.SetDirty(LevelLoader.GetContainer());
            AssetDatabase.SaveAssets();
        }

        public async UniTask<bool> CheckToRefreshLevel() {
            if (IsDirty()) {
                return true;
            }
            return false;
        }

        public void ClickNew(EditInspector setting) {
            throw new NotImplementedException();
        }

        public void ResetLevel() {
            StartLevel1();
        }

        public void SetColorCandidates(List<ColorIndex> colorCandidates) {
            _contorller.SetColorCandidates(colorCandidates);
        }
    }
}