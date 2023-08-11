using Cysharp.Threading.Tasks;
using UnityEngine;

namespace GemMatch {
    public class EntityView : MonoBehaviour {
        public Entity Entity { get; private set; }
        public TileView TileView { get; private set; }

        public virtual void Initialize(TileView tileView, Entity entity = null) {
            TileView = tileView;
            Entity = entity ?? Entity;
        }

        public virtual async UniTask OnCreate() { }

        public virtual async UniTask OnUpdate() { }

        public virtual async UniTask DestroyAsync(bool isImmediately) {
            DestroyImmediate(gameObject);
        }
        
        public virtual async UniTask OnMoveMemory() { }
    }
}