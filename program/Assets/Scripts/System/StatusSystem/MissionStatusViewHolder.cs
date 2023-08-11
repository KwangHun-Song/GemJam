using System.Collections.Generic;
using GemMatch;
using UnityEngine;

namespace OverlayStatusSystem {
    public class MissionStatusViewHolder : MonoBehaviour {
        [SerializeField] private RectTransform missionRoot;
        [SerializeField] private GameObject missionPrefab;
        [SerializeField] private List<MissionStatusView> missions;

        private void OnDisable() {
            foreach (MissionStatusView view in missions) {
                Destroy(view.gameObject);
            }
            missions.Clear();
        }

        public void InitializeMissions(Mission[] targetMissions) {
            foreach (var m in targetMissions) {
                var missionView = Instantiate(missionPrefab, missionRoot).GetComponent<MissionStatusView>();
                missionView.name = $"Mission({m.entity.index},{m.entity.color})";
                missions.Add(missionView);
                missionView.InitializeMission(m);
            }
        }

        public void AchieveMission(Mission mission, int changeCount) {
            foreach (var view in missions) {
                view.GetMissionAsync(mission, changeCount).Forget();
            }
        }
    }
}