using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using UnityEngine.UI;

namespace GemMatch {
    public class Touchable {
        private readonly Button oneClickButton;

        public Touchable(Button oneClickButton) {
            this.oneClickButton = oneClickButton;
        }

        public void Register(ITouchable subject, ITouchableListener listener) {
            oneClickButton.OnClickAsAsyncEnumerable().Take(1).ForEachAsync(_ => {
                OnTouch(listener);
            });

            // todo: 인터렉션이 될때 상호작용할 상태
            void OnTouch(ITouchableListener listener) {
                // todo: 알려줘야할 상태를 매개변수에 넣는다. 임시로 this 넣음
                listener.NotifyOnTouch(subject);
            }
        }
    }
}