using System;
using Cysharp.Threading.Tasks;
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
            
            Controller = StartGame(Param.levelIndex);
            
            foreach (var selectedBooster in Param.selectedBoosters) {
                switch (selectedBooster) {
                    case BoosterIndex.ReadyBoosterRocket:
                        Controller.InputAbility(new RocketAbility(Controller));
                        break;
                    case BoosterIndex.ReadyBoosterExtraSlot:
                        break;
                }
            }
        }

        public Controller StartGame(int levelIndex) {
            var controller = new Controller();
            controller.Listeners.Add(view);

            if (FindObjectOfType<EditLevelIndicator>() is EditLevelIndicator indicator && indicator != null) {
                levelIndex = indicator.LevelIndex;
            }
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

        private int levelIndexInput;
        public string LevelIndexInput {
            set {
                if (int.TryParse(value, out var levelIndex)) {
                    levelIndexInput = levelIndex;
                }
            }
        }
        
        public void OnClickStartGame() {
            StartGame(levelIndexInput);    
        }

        #endregion
    }
}