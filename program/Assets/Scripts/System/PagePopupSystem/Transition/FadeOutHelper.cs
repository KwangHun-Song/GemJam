using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PagePopupSystem {
    public static class FadeOutHelper {
        private static FadeTransition transition;
        private static FadeTransition Transition => transition ??= Object.Instantiate(Resources.Load<FadeTransition>(nameof(FadeTransition)));

        public static UniTask FadeOut() => Transition.FadeOut();
        public static UniTask FadeIn() => Transition.FadeIn();
    }
}