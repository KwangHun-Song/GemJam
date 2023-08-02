using Cysharp.Threading.Tasks.Linq;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;

namespace GemMatch {
    public class Collidable {
        private readonly Collider2D collider;

        public Collidable(Collider2D collider) {
            this.collider = collider;
        }

        public void Register(IColliderable subject, ICollidableListener listener) {
            collider.GetAsyncTriggerEnter2DTrigger().ForEachAsync(_ => {
                OnCollide(listener);
            });

            void OnCollide(ICollidableListener listener) {
                // todo: 알려줘야할 상태를 매개변수에 넣는다. 임시로 this 넣음
                listener.NotifyOnCollide(subject);
            }
        }
    }
}