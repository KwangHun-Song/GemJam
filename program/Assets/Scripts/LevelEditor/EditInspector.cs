using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using PagePopupSystem;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GemMatch.LevelEditor {
    public class EditInspector : MonoBehaviour {
#region Public Property
        public int LevelIndex {
            get => PlayerPrefs.GetInt(Constants.LevelIndexPrefsKey, 0);
            set {
                PlayerPrefs.SetInt(Constants.LevelIndexPrefsKey, value);
                PlayerPrefs.Save();
            }
        }

        public string SavePath { // txt파일로 뽑기 위한 준비
            get => PlayerPrefs.GetString("EDIT_SAVE_PATH", "Assets/Data/text");
            set => PlayerPrefs.SetString("EDIT_SAVE_PATH", value);
        }

        public int GemCount { get; private set; } = 0;

#endregion
        private IEditInspectorEventListener _contorller;
        private EditLevelValidator _validator;

        public void RefreshGemCount() {
            this.GemCount = _contorller.CurrentLevel.tiles
                .Where(t=>t.entityModels.Count > 0)
                .Select(t => t.EntityDict[Layer.Piece])
                .Where(t=>ModelTemplates.AllColors.Contains(t.Color))
                .Count();
        }

#if UNITY_EDITOR
        public void SetDirty() => EditorUtility.SetDirty(this.gameObject);
        private bool IsDirty() => EditorUtility.IsDirty(this.gameObject);
#endif

        private void Start() {
            PageManager.ChangeTo(Page.EditPage);
        }

        public void Initialize(IEditInspectorEventListener gameController) {
            this._contorller = gameController;
            this._validator = new EditLevelValidator();
        }

        public void LoadLevel(int levelIndex) {
            var levelsLength = LevelLoader.GetContainer().levels.Length;
            var lastIndex = Math.Min(levelIndex, levelsLength - 1);
            LevelIndex = lastIndex;
            var levelCache = LevelLoader.GetLevel(LevelIndex).Clone();
            _contorller.LoadLevel(levelCache);
            OnLoadLevel?.Invoke(this);
            LevelIndex = lastIndex;
            RefreshGemCount();
        }

        public void NextLevel() {
            if (LevelLoader.GetContainer().levels.Length <= LevelIndex) return;
            LoadLevel(LevelIndex + 1);
        }

        public void PrevLevel() {
            if (LevelIndex <= 0) return;
            LoadLevel(LevelIndex - 1);
        }

        public void NewLevel() {
            LevelIndex = LevelLoader.GetContainer().levels.Length;
            _contorller.MakeLevel1();
            var cache = new List<Level>(LevelLoader.GetContainer().levels);
            cache.Add(_contorller.CurrentLevel);
            RefreshGemCount();
            LevelLoader.GetContainer().levels = cache.ToArray();
#if UNITY_EDITOR
            SetDirty();
            EditorUtility.SetDirty(LevelLoader.GetContainer());
            AssetDatabase.SaveAssets();
#endif
        }

        public void ResetLevel() {
            LoadLevel(LevelIndex);
        }

        public void SaveLevel() {
            var lvsCache = LevelLoader.GetContainer().levels.Select(l=>l.Clone()).ToList();
            if (lvsCache.Count <= LevelIndex) {
                lvsCache.Add(_contorller.CurrentLevel);
            } else {
                lvsCache[LevelIndex] = _contorller.CurrentLevel;
            }
            LevelLoader.GetContainer().levels = lvsCache.ToArray();
#if UNITY_EDITOR
            SetDirty();
            EditorUtility.SetDirty(LevelLoader.GetContainer());
            AssetDatabase.SaveAssets();
#endif
        }

        public void DeleteLevel() {
            var cache = LevelLoader.GetContainer().levels
                .Select(l => l.Clone())
                .ToList();
            cache.RemoveAt(LevelIndex);
            LevelLoader.GetContainer().levels = cache.ToArray();
#if UNITY_EDITOR
            SetDirty();
            EditorUtility.SetDirty(LevelLoader.GetContainer());
            AssetDatabase.SaveAssets();
#endif
        }

        public event Action<EditInspector> OnLoadLevel;

        public async UniTask<bool> ExecuteWhenUpdateLevel(System.Action<EditInspector> callback) {
#if UNITY_EDITOR
            if (IsDirty()) {
                callback?.Invoke(this);
                return true;
            }
#endif
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

        public int GetColorCount() {
            if (_contorller?.CurrentLevel == null) return -1;
            return _contorller.CurrentLevel.colorCount;
        }

        public void SetColorCount(int colorCount) {
            _contorller.CurrentLevel.colorCount = colorCount;
        }

        public bool ValidateCurrentLevel(int inputColorCount, out int? validColorCount) {
            bool result = _validator.Validate(_contorller.CurrentLevel, inputColorCount);
            validColorCount = _validator.GetColorCountCachedOnBoard();
            return result;
        }

        public void SetBoardSize(int boardHeight, int boardWidth) {
            _contorller.ResizeBoard(boardHeight, boardWidth);
        }
    }

    internal class EditLevelValidator {
        private int validColorCached = -1;
        public bool Validate(Level level, int colorCount) {
            if (level?.tiles == null || level?.colorCandidates == null) return false;
            var colorOnBoard = level.tiles
                .SelectMany(t => t.entityModels)
                .Where(m => m.index == EntityIndex.NormalPiece && Constants.UsableColors.Contains(m.color))
                .Select(m => m.color)
                .Distinct();
            validColorCached = colorOnBoard.Union(level.colorCandidates).Count();
            return colorCount == validColorCached;
        }

        public int GetColorCountCachedOnBoard() => validColorCached;
    }
}