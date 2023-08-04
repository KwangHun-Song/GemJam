using System.Collections.Generic;
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

        public static T PickRandom<T>(this IList<T> collection) {
            return collection[UnityEngine.Random.Range(0, collection.Count)];
        }
    }
}