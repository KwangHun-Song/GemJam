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

        public Mission mission;
        private EntityModel targetEntityModel;

        public async UniTaskVoid GetMissionAsync(Mission mission, int changeCount) {
            if (IsMyModel(mission.entity) == false) return;
            // input 되는 대상을 나중에 OnSave 시 OnMission 매개변수로 넘긴다
            object param = new ArrayList(){
                mission.entity.Clone(),
                changeCount
            };
            OverlayStatusHelper.Input(this, new OverlayStatusParam(param));
            await UniTask.Delay(1000); // / 몬가 애니메이션 연출을 여기 넣는다
            OverlayStatusHelper.Save(this);
        }

        public void OnMission(ArrayList missionParam) {
            var entityModel = (EntityModel)missionParam[0];
            var changeCount = (int)missionParam[1];
            if (IsMyModel(entityModel) == false) return;
            this.mission.count = changeCount;
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
            public MissionOverlayStatus(IOverlayStatusEvent missionStatusView, Action<ArrayList> onMission) : base(missionStatusView, onMission) {
            }

            public override void Save() {
                while (EventRecord.Count > 0) {
                    OnEvent?.Invoke((ArrayList)EventRecord.Dequeue().Value);
                }
            }
        }
    }
}