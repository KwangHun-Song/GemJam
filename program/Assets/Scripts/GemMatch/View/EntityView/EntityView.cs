using Cysharp.Threading.Tasks;
using UnityEngine;

namespace GemMatch {
    public class EntityView : MonoBehaviour {
        public Entity Entity { get; private set; }
        public TileView TileView { get; private set; }

        public virtual void Initialize(TileView tileView, Entity entity) {
            TileView = tileView;
            Entity = entity;
        }

        public virtual async UniTask OnCreate() { }

        public virtual async UniTask DestroyAsync() {
            DestroyImmediate(gameObject);
        }
    }
}