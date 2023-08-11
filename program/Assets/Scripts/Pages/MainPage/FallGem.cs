using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Pages {
    [RequireComponent(typeof(Image))]
    public class FallGem : MonoBehaviour {
        [SerializeField] private Sprite[] sprites;

        public void Awake() {
            GetComponent<Image>().sprite = sprites[Random.Range(0, sprites.Length)];
        }
    }
}