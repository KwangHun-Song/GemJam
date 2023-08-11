using System.Collections;
using GemMatch;
using PagePopupSystem;
using UnityEngine;

namespace OverlayStatusSystem {
    public class OverlayStatusHolder : MonoBehaviour{
        [SerializeField] private CoinStatusView _coinStatus;
        [SerializeField] private MissionStatusViewHolder _missionStatus;
        [SerializeField] private LevelStatusView _levelStatus;

        public static OverlayStatusHolder Instance = null;

        private void Awake() {
            PageManager.OnPageChanged -= GetStable;
            PageManager.OnPageChanged += GetStable;
        }

        private IEnumerator Start() {
            yield return null;
            GetStable(PageManager.CurrentPage);
            yield return null;
            Instance = this;
        }

        private void GetStable(Page currentPage) {
            _coinStatus.gameObject.SetActive(true);
            _missionStatus.gameObject.SetActive(currentPage == Page.PlayPage);
            _levelStatus.gameObject.SetActive(currentPage == Page.PlayPage);
        }

        public void InitializeMission(Mission[] targetMissions) => _missionStatus.InitializeMissions(targetMissions);

        public void AchieveMissionCount(Mission mission, int changeCount) {
            _missionStatus.AchieveMission(mission, changeCount);
        }
    }
}