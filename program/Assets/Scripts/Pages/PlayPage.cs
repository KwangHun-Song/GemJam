using System;
using GemMatch;
using GemMatch.LevelEditor;
using PagePopupSystem;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace Pages {
    public class PlayPageParam {
        public int levelIndex;
        public BoosterIndex[] selectedBoosters;
    }
    
    public class PlayPage : PageHandler {
        [SerializeField] private View view;
        
        public override Page GetPageType() => Page.PlayPage;
        public Controller Controller { get; private set; }
        public PlayPageParam Param { get; private set; }

        public override void OnWillEnter(object param) {
            Assert.IsTrue(param is PlayPageParam);
            Param = (PlayPageParam)param;

            var levelIndex = Param.levelIndex;
            if (FindObjectOfType<EditLevelIndicator>() is EditLevelIndicator indicator && indicator != null) {
                levelIndex = indicator.LevelIndex;
            }
            
            Controller = StartGame(levelIndex);
            
            foreach (var selectedBooster in Param.selectedBoosters) {
                switch (selectedBooster) {
                    case BoosterIndex.ReadyBoosterRocket:
                        Controller.InputAbility(new RocketAbility(Controller));
                        Controller.UndoHandler.Reset();
                        break;
                    case BoosterIndex.ReadyBoosterExtraSlot:
                        Controller.AddExtraMemorySlot();
                        break;
                }
            }
        }

        public Controller StartGame(int levelIndex) {
            var controller = new Controller();
            controller.Listeners.Add(view);

            var level = LevelLoader.GetLevel(levelIndex);
            
            controller.StartGame(level);
            return controller;
        }

        #region EVENT

        public void OnClickBack() {
            ChangeTo(Page.MainPage);
        }

        #endregion

        #region Booster

        public void OnClickUndo() {
            if (Controller != null) {
                if (Controller.UndoHandler.IsEmpty()) {
                    Debug.Log($"Is Empty");
                    return;
                }
                Controller.UndoHandler.Undo();
            }
        }

        public void OnClickMagnet() {
            Controller?.InputAbility(new MagneticAbility(Controller));
        }

        public void OnClickShuffle() {
            Controller?.InputAbility(new ShuffleAbility(Controller));
        }

        #endregion

        private void Update() {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                GoBackToEditMode();
            }
        }
        

        private void GoBackToEditMode() {
            if (FindObjectOfType<EditLevelIndicator>() != null)
                SceneManager.LoadScene("EditScene");
        }

        #region CHEAT
        
        public void OnClickStartGame() {
            StartGame(Param.levelIndex);    
        }

        public void OnClickPrev() {
            Param.levelIndex = Mathf.Clamp(Param.levelIndex - 1, 0, LevelLoader.GetContainer().levels.Length - 1);
            StartGame(Param.levelIndex);    
        }

        public void OnClickNext() {
            Param.levelIndex = Mathf.Clamp(Param.levelIndex + 1, 0, LevelLoader.GetContainer().levels.Length - 1);
            StartGame(Param.levelIndex);    
        }

        #endregion
    }
}