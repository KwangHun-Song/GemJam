using UnityEngine;
using UnityEngine.EventSystems;

namespace GemMatch.LevelEditor {
    public class EditGameBoard : UIBehaviour {
        [SerializeField] private CurrentGameObject currentTileView;
        [SerializeField] private CurrentGameObject currentEntityView;

        public void Repaint() {
            currentTileView.Repaint();
        }

        internal class CurrentGameObject : MonoBehaviour {
            public RectTransform RectTransform;
            public Transform Transform;
            public GameObject GameObject;
            public SpriteRenderer SpriteRenderer;

            private void Awake() {
                this.Transform = transform;
                this.RectTransform = GetComponent<RectTransform>();
                this.SpriteRenderer = GetComponent<SpriteRenderer>();
                this.GameObject = gameObject;
            }

            public void Repaint() {
                // todo: 데이터에 따라 랜더러 바꾸기

            }
        }
    }
}