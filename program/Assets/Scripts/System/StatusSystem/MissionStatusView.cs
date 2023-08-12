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
        [SerializeField] private Sprite[] sprites;
        [SerializeField] private ParticleSystem crashParticle;
        [SerializeField] private GameObject normalPiecePrefab;
        public Transform collectionRoot;

        public Mission mission;
        private EntityModel targetEntityModel;

        public async UniTask GetMissionAsync(Mission targetMission, int changeCount) {
            if (IsMyModel(targetMission.entity) == false) return;
            crashParticle.Play();
            OverlayStatusHelper.Save(this);
            txtCount.text = $"{changeCount}";
        }

        public void OnMission(ArrayList missionParam) {
            var entityModel = (EntityModel)missionParam[0];
            var reduceCount = (int)missionParam[1]; // 1이 들어온다
            if (IsMyModel(entityModel) == false) return;
            this.mission.count -= reduceCount;
            txtCount.text = $"{this.mission.count}";
        }

        private bool IsMyModel(EntityModel entityModel) {
            if (this.targetEntityModel == null) return false;
            if (entityModel.index != this.targetEntityModel.index) return false;
            if (entityModel.color != this.targetEntityModel.color) return false;
            return true;
        }

        public Type GetKeyType() => typeof(MissionOverlayStatus);

        // mission 정보를 초기화
        public void InitializeMission(Mission mission) {
            this.mission = mission;
            this.targetEntityModel = mission.entity;
            OverlayStatusHelper.Init(new MissionOverlayStatus(this, OnMission));
            if (targetEntityModel.index == EntityIndex.NormalPiece) {
                var index = targetEntityModel.color == ColorIndex.Random ? sprites.Length-1 : (int)targetEntityModel.color;
                imgMission.sprite = sprites[index];
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