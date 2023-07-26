using Cysharp.Threading.Tasks;
using UnityEngine;

namespace GemMatch {
    public class EntityView : MonoBehaviour {
        public Entity Entity { get; private set; }

        public virtual void Initialize(Entity entity) {
            Entity = entity;
        }

        public virtual async UniTask DestroyAsync() {
            Destroy(this);
        }
    }
}