using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace GemMatch.LevelEditor {
    public interface IEditCtrlForInspector {
    }

    public class EditInspector : MonoBehaviour, IEditCtrlForInspector {
#region Public Property
        public int LevelIndex {
            get => PlayerPrefs.GetInt("LAST_INDEX", 0);
            set {
                PlayerPrefs.SetInt("LAST_INDEX", value);
                PlayerPrefs.Save();
            }
        }

        public string SavePath { // txt파일로 뽑기 위한 준비
            get => PlayerPrefs.GetString("EDIT_SAVE_PATH", "Assets/Data/text");
            set => PlayerPrefs.SetString("EDIT_SAVE_PATH", value);
        }
#endregion
        private IEditInspectorEventListener _contorller;

        public void SetDirty() => EditorUtility.SetDirty(this.gameObject);
        private bool IsDirty() => EditorUtility.IsDirty(this.gameObject);

        public void Initialize(IEditInspectorEventListener gameController) {
            this._contorller = gameController;
        }

        public void LoadLevel(int levelIndex) {
            var levelsLength = LevelLoader.GetContainer().levels.Length;
            LevelIndex = Math.Min(levelIndex, levelsLength - 1);
            var levelCache = LevelLoader.GetLevel(LevelIndex).Clone();
            _contorller.LoadLevel(levelCache);
            OnLoadLevel?.Invoke(this);
        }

        public void NewLevel() {
            LevelIndex = LevelLoader.GetContainer().levels.Length;
            _contorller.MakeLevel1();
        }

        public void ResetLevel() {
            LoadLevel(LevelIndex);
        }

        public void SaveLevel() {
            var lvsCache = new List<Level>(LevelLoader.GetContainer().levels);
            if (lvsCache.Count <= LevelIndex) {
                lvsCache.Add(_contorller.CurrentLevel);
            } else {
                lvsCache[LevelIndex] = _contorller.CurrentLevel;
            }
            LevelLoader.GetContainer().levels = lvsCache.ToArray();
            SetDirty();
            EditorUtility.SetDirty(LevelLoader.GetContainer());
            AssetDatabase.SaveAssets();
        }

        public event Action<EditInspector> OnLoadLevel;

        public async UniTask<bool> ExecuteWhenUpdateLevel(System.Action<EditInspector> callback) {
            if (IsDirty()) {
                callback?.Invoke(this);
                return true;
            }
            return false;
        }

        public void SetColorCandidates(IEnumerable<ColorIndex> colorCandidates) {
            _contorller.SetColorCandidates(colorCandidates.ToList());
        }

        public IEnumerable<ColorIndex> GetColorCandidates() => _contorller.CurrentLevel.colorCandidates;

        public void SetMissions(List<Mission> missions) {
            _contorller.SetMissions(missions);
        }

        public IEnumerable<Mission> GetMissions() => _contorller.CurrentLevel.missions;

    }
}