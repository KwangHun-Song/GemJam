using System.Collections;
using Cysharp.Threading.Tasks;
using GemMatch;
using PagePopupSystem;
using Record;
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

        public void UpdateMissionCount(Mission mission, int changeCount) {
            _missionStatus.AchieveMissionAsync(mission, changeCount).Forget();
        }

        public void CollectMissionViewClones(EntityModel entityModel, GameObject entityViewGameObject) {
            _missionStatus.CollectMissionViewClones(entityModel, entityViewGameObject);
        }

        public void UpdateLevelStatus(Level currentLevel) {
            _levelStatus.UpdateLevel(PlayerInfo.HighestClearedLevelIndex + 1);
        }
    }
}