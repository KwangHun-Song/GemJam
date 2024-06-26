using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using GemMatch;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OverlayStatusSystem {
    public class MissionStatusView : MonoBehaviour, IOverlayStatusEvent {
        [SerializeField] private TMP_Text txtCount;
        [SerializeField] private Image imgMission;
        [SerializeField] private Image imgCheck;
        [SerializeField] private Sprite[] sprites;
        [SerializeField] private Sprite allColorNormalPieceSprite;
        [SerializeField] private GameObject normalPiecePrefab;
        [SerializeField] private GameObject crashPrefab;

        public Transform CollectionRoot;

        public Mission mission;
        private EntityModel targetEntityModel;

        public async UniTask GetMissionAsync(Mission targetMission, int changeCount) {
            if (IsMyModel(targetMission.entity) == false) return;
            ShowCrashFXAsync().Forget();
            OverlayStatusHelper.Save(this);
            txtCount.text = $"{changeCount}";
            EnableMissionCount();
        }

        public void OnMission(ArrayList missionParam) {
            var entityModel = (EntityModel)missionParam[0];
            var reduceCount = (int)missionParam[1]; // 1이 들어온다
            if (IsMyModel(entityModel) == false) return;
            this.mission.count -= reduceCount;
            txtCount.text = $"{this.mission.count}";
            EnableMissionCount();
        }

        private async UniTaskVoid ShowCrashFXAsync() {
            var crashFX = GameObject.Instantiate(crashPrefab, CollectionRoot);
            await UniTask.Delay(TimeSpan.FromSeconds(2f));
            GameObject.Destroy(crashFX.gameObject);
        }

        private bool IsMyModel(EntityModel entityModel) {
            if (this.targetEntityModel == null) return false;
            if (entityModel.index != this.targetEntityModel.index) return false;
            if (entityModel.color != ColorIndex.All && 
                entityModel.color != this.targetEntityModel.color) return false;
            return true;
        }

        public Type GetKeyType() => typeof(MissionOverlayStatus);

        private void EnableMissionCount() {
            txtCount.gameObject.SetActive(mission.count > 0);
            imgCheck.gameObject.SetActive(mission.count <= 0);
        }

        // mission 정보를 초기화
        public void InitializeMission(Mission mission) {
            this.mission = mission;
            this.targetEntityModel = mission.entity;
            EnableMissionCount();
            OverlayStatusHelper.Init(new MissionOverlayStatus(this, OnMission));
            if (targetEntityModel.index == EntityIndex.NormalPiece) {
                if (targetEntityModel.color == ColorIndex.All) {
                    imgMission.sprite = allColorNormalPieceSprite;
                } else {
                    var index = targetEntityModel.color == ColorIndex.Random ? sprites.Length-1 : (int)targetEntityModel.color;
                    imgMission.sprite = sprites[index];
                }
            } else if (targetEntityModel.index == EntityIndex.GoalPiece) {
                imgMission.sprite = sprites[sprites.Length-1];
            }
            txtCount.text = $"{mission.count}";
        }

        public class MissionOverlayStatus : OverlayStatus<ArrayList> {
            private MissionStatusView missionStatusView;
            public MissionOverlayStatus(IOverlayStatusEvent missionStatusView, Action<ArrayList> onMission) : base(missionStatusView, onMission) {
                this.missionStatusView = (MissionStatusView)missionStatusView;
            }

            public override void Save() {
                while (EventRecord.Count > 0) {
                    OnEvent?.Invoke((ArrayList)EventRecord.Dequeue().Value);
                }
            }
        }
    }
}