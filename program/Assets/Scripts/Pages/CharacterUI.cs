using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Utility;

namespace Pages {
    public class CharacterUI : MonoBehaviour {
        private static int DoggyWalk = Animator.StringToHash($"dogMove");
        private static int DoggyStopMoving = Animator.StringToHash($"dogStopMoving");
        private static int DoggyClear = Animator.StringToHash($"dogClear");
        private static int DoggyGoal = Animator.StringToHash($"dogGoal");
        private static int DoggyIdle = Animator.StringToHash($"dogIdle001");

        private static int GnomeWalk = Animator.StringToHash($"gnomeMove");
        private static int GnomeStopMoving = Animator.StringToHash($"gnomeStopMoving");
        private static int GnomeClear = Animator.StringToHash($"gnomeClear");
        private static int GnomeIdle = Animator.StringToHash($"gnomeIdle001");
        private static int GnomeGoal = Animator.StringToHash($"gnomeGoal");

        [SerializeField] private Animator gnomeAni;
        [SerializeField] private Animator doggyAni;

        public void WalkIn() {
            doggyAni.SetTrigger(DoggyWalk);
            gnomeAni.SetTrigger(GnomeWalk);
        }

        public void ClearPopup() {
            doggyAni.SetTrigger(DoggyClear);
            gnomeAni.SetTrigger(GnomeClear);
        }

        public void StopWalking() {
            doggyAni.SetTrigger(DoggyStopMoving);
            gnomeAni.SetTrigger(GnomeStopMoving);
        }

        public void SetIdle() {
            doggyAni.SetTrigger(DoggyIdle);
            gnomeAni.SetTrigger(GnomeIdle);
        }
    }
}