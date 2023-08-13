using UnityEngine;

namespace Pages {
    public class CharacterUI : MonoBehaviour {
        private static int DoggyWalk = Animator.StringToHash($"dogMove");
        private static int DoggyStopMoving = Animator.StringToHash($"dogStopMoving");
        private static int DoggyClear = Animator.StringToHash($"dogClear");
        private static int DoggyGoal = Animator.StringToHash($"dogGoal");

        private static int GnomeWalk = Animator.StringToHash($"gnomeMove");
        private static int GnomeStopMoving = Animator.StringToHash($"gnomeStopMoving");
        private static int GnomeClear = Animator.StringToHash($"gnomeClear");
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
    }
}