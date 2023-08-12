using System.Collections.Generic;
using GemMatch;
using UnityEngine;

namespace OverlayStatusSystem {
    public class MissionStatusViewHolder : MonoBehaviour {
        [SerializeField] private RectTransform missionRoot;
        [SerializeField] private GameObject missionPrefab;
        [SerializeField] private List<MissionStatusView> missions;

        private void OnEnable() {
            Instance = this;
        }

        private void OnDisable() {
            Instance = null;
            ClearMissionViews();
        }

        public void InitializeMissions(Mission[] targetMissions) {
            if (missions.Count > 0) {
                ClearMissionViews();
            }

            foreach (var m in targetMissions) {
                var missionView = Instantiate(missionPrefab, missionRoot).GetComponent<MissionStatusView>();
                missionView.name = $"Mission({m.entity.index},{m.entity.color})";
                missions.Add(missionView);
                missionView.InitializeMission(m);
            }
        }

        private void ClearMissionViews() {
            foreach (MissionStatusView view in missions) {
                Destroy(view.gameObject);
            }
            missions.Clear();
        }

        public void AchieveMission(Mission mission, int changeCount) {
            foreach (var view in missions) {
                view.GetMissionAsync(mission, changeCount).Forget();
            }
        }

        private static MissionStatusViewHolder Instance = null;
        public static Transform GetMissionStatusViewPosition(EntityModel targetEntity) {
            if (Instance == null || Instance.missions.Count == 0) return null;

            var statusView = Instance!.missions!.SingleOrDefault(m=>m.mission.entity.index == targetEntity.index && m.mission.entity.color == targetEntity.color);

            if (statusView == null) return null;
            return statusView.transform;
        }
    }
}