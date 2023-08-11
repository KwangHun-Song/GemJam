using System;
using System.Collections;
using System.Runtime.CompilerServices;
using GemMatch;
using PagePopupSystem;
using UnityEngine;

namespace OverlayStatusSystem {
    public class OverlayStatusHolder : MonoBehaviour{
        [SerializeField] private CoinStatusView _coinStatus;
        [SerializeField] private MissionStatusView _missionStatus;
        [SerializeField] private LevelStatusView _levelStatus;

        public static OverlayStatusHolder Instance = null;

        private IEnumerator Start() {
            yield return null;
            GetStable();
            yield return null;
            Instance = this;
        }

        private void GetStable() {
            _coinStatus.gameObject.SetActive(true);
            _missionStatus.gameObject.SetActive(PageManager.CurrentPage == Page.PlayPage);
            _levelStatus.gameObject.SetActive(PageManager.CurrentPage == Page.PlayPage);
        }

        public void InitializeMission(Mission[] targetMissions) => _missionStatus.InitializeMissions(targetMissions);
    }
}