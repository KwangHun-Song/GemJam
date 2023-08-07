using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace GemMatch {
    public static class Utility {
        public static UniTask ToUniTask(this Tween tween) {
            if (tween == null || tween.IsComplete()) 
                return UniTask.CompletedTask;

            var source = new UniTaskCompletionSource<bool>();
            tween.OnComplete(() => { source.TrySetResult(true); });

            return source.Task;
        }

        public static T PickRandom<T>(this IEnumerable<T> collection) {
            return collection.ElementAt(UnityEngine.Random.Range(0, collection.Count()));
        }

        public static Entity GetEntity(EntityModel entityModel) {
            return entityModel.index switch {
                EntityIndex.NormalPiece => new NormalPiece(entityModel),
            };
        }
    }
}