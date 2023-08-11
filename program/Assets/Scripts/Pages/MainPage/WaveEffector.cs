using UnityEngine;

namespace Pages {
    public class WaveEffector : MonoBehaviour {
        [SerializeField] private WaveEffect[] waves;
        [SerializeField] private float duration;
        [SerializeField] private float interval;

        private void Start() {
            for (int i = 0; i < waves.Length; i++) {
                waves[i].StartEffect(duration, interval * i, interval * waves.Length - duration);
            }
        }
    }
}