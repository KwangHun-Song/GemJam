using System;
using Cysharp.Threading.Tasks;
using GemMatch;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OverlayStatusSystem {
    public class MissionView : MonoBehaviour, IOverlayStatusEvent {
        [SerializeField] private TMP_Text txtCount;
        [SerializeField] private Image imgMission;
        [SerializeField] private Sprite[] sprites;

        private Mission mission;
        private EntityModel targetEntityModel;

        private void Start() {
            OverlayStatusHelper.Init(new MissionOverlayStatus(this, OnMission).SetTargetModel(targetEntityModel));
        }

        public async UniTaskVoid GetMission(Mission mission) {
            if (IsMyModel(mission.entity) == false) return;
            OverlayStatusHelper.Input(this, new OverlayStatusParam(targetEntityModel.Clone()));
            await UniTask.Delay(1000); // / 몬가 애니메이션 연출을 여기 넣는다
            OverlayStatusHelper.Save(this);
        }

        public void OnMission(EntityModel entityModel) {
            if (IsMyModel(entityModel) == false) return;
            this.mission.count--;
            txtCount.text = $"{this.mission.count}";
        }

        private bool IsMyModel(EntityModel entityModel) {
            if (entityModel.index != this.targetEntityModel.index) return false;
            if (entityModel.color != this.targetEntityModel.color) return false;
            return true;
        }

        public Type GetKeyType() => typeof(MissionOverlayStatus);

        // mission 정보를 초기화
        public void InitializeMission(Mission mission) {
            this.mission = mission;
            this.targetEntityModel = mission.entity;
            if (targetEntityModel.index == EntityIndex.NormalPiece) {
                var index = targetEntityModel.color == ColorIndex.Random ? sprites.Length-1 : (int)targetEntityModel.color;
                imgMission.sprite = sprites[index];
            } else if (targetEntityModel.index == EntityIndex.GoalPiece) {
                imgMission.sprite = sprites[sprites.Length-1];
            }
        }
    }
}