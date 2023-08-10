using System.Collections.Generic;
using GemMatch;
using UnityEngine;

namespace OverlayStatusSystem {
    public class MissionStatusView : MonoBehaviour {
        [SerializeField] private List<MissionView> missions;

        public void InitializeMissions(Mission[] targetMissions) {
            for (var i = 0; i < missions.Count; i++) {
                if (targetMissions.Length <= i) {
                    missions[i].gameObject.SetActive(false);
                    continue;
                }
                missions[i].gameObject.SetActive(true);
                missions[i].InitializeMission(targetMissions[i]);
            }
        }

        public void GetMission(Mission mission) {
            foreach (var view in missions) {
                view.GetMission(mission);
            }
        }
    }
}