using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using GemMatch;
using Record;
using TMPro;
using UnityEngine;

namespace OverlayStatusSystem {
    public class LevelStatusView : OverlayStatusEvent<LevelOverlayStatus> {
        [SerializeField] private TMP_Text levelNumber;
        [SerializeField] private Animator[] levelIndexers;
        [SerializeField] private GameObject[] levelIndexerRoots;

        private int PlayerInfoIndex => PlayerInfo.HighestClearedLevelIndex % 3;
        private int stageIndex = 0;

        public void Start() {
            OverlayStatusHelper.Init(new LevelOverlayStatus(this, OnStage));
            foreach (var root in levelIndexerRoots) {
                root.SetActive(false);
            }

            foreach (var anim in levelIndexers) {
                anim.SetTrigger("00idle");
            }
        }

        private void OnEnable() {
            // levelNumber.text = $"{PlayerInfo.HighestClearedLevelIndex + 1}";
            // stageIndex = PlayerInfoIndex;
        }


        private int currentStage = 0;
        public async UniTaskVoid UpdateLevel(int stage) {
            currentStage = stage;
            levelNumber.text = $"{stage}";
            for (var i = 0; i < levelIndexerRoots.Length; i++) {
                var root = levelIndexerRoots[i];
                root.SetActive(i == PlayerInfoIndex);
            }
            await base.Get<int>(PlayerInfoIndex + 1);
        }

        public override async UniTask Animate() {
            var targetAnim = levelIndexers.Where(i => i.name.Equals($"level_map0{PlayerInfoIndex+1}"));
            foreach (var anim in targetAnim) {
                await UniTask.DelayFrame(6);
                anim.SetTrigger("01");
            }
            await UniTask.DelayFrame(5);
        }

        private void OnStage(int stage) { } // Save 뒤 할일이 없을듯
    }

    public class LevelOverlayStatus : OverlayStatus<int> {
        public LevelOverlayStatus(IOverlayStatusEvent levelStatusView, Action<int> onStage) : base (levelStatusView, onStage) {
        }

        public override void Save() {
            while (EventRecord.Count > 0) {
                EventRecord.Dequeue();
                OnEvent?.Invoke(1); // stage는 1개씩 클리어 고정
            }
        }
    }
}