using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using Button = UnityEngine.UI.Button;

namespace GemMatch.LevelEditor.ActionEvent {
    /// <summary>
    /// UniTask로 이벤트를 구독시켜서 View와 MonoBehaviour 바인딩
    /// </summary>

    public static class EventLinker {
        // Inner class
        private class InnerEventLinker {//: IDisposable {
            private readonly UIBehaviour subject;

            public InnerEventLinker(UIBehaviour subject) {
                this.subject = subject;
                DisposeOnDestroyAsync().Forget();
            }

            public bool CheckTarget() => subject != null;

            private readonly CancellationTokenSource cancelsrc = new CancellationTokenSource();

            public InnerEventLinker BindButton() {
                if (false == (this.subject is Button)) return null;
                BindButtonAsync().Forget();
                return this;
            }

            public bool Match(UIBehaviour candidate) => this.subject == candidate;

            public void Dispose() {
                cancelsrc.Cancel();
            }

            // todo: component 종류에 따라 구현을 추가한다
            private async UniTaskVoid BindButtonAsync() {
                await (this.subject as Button).OnClickAsAsyncEnumerable().ForEachAsync(_ => {
                    EventLinker.onNotifyForButton?.Invoke();
                }, cancelsrc.Token);
                Debug.Log("End BindButtonAsync"); // todo: 여기 왜 안 들어올까?
            }

            private async UniTask DisposeOnDestroyAsync() {
                await subject.GetAsyncDestroyTrigger().OnDestroyAsync();
                Dispose(); // 여긴 들어오는데
            }
        }

        // Ievnetsubejct 구체 type으로 구분한다
        private static readonly Dictionary<Type, List<InnerEventLinker>> register = new Dictionary<Type, List<InnerEventLinker>>();

        public delegate void OnNotify();
        private static OnNotify onNotifyForButton;

        public static void Bind<TSubject>(TSubject subject) where TSubject : UIBehaviour {
            Debug.Assert(subject != null);
            if (register.ContainsKey(typeof(TSubject)) == false) {
                register.Add(typeof(TSubject), new List<InnerEventLinker>());
            }
            register[typeof(TSubject)].Add(new InnerEventLinker(subject).BindButton());
        }

        public static void Unbind<TSubject>(TSubject subject) where TSubject : UIBehaviour {
            Debug.Assert(subject != null);
            if (register.ContainsKey(typeof(TSubject)) == false) return;
            var result = register[typeof(TSubject)].Where(l => {
                return l != null && l.CheckTarget() && l.Match(subject) == false;
            }).ToList();
            if (result.Count == 0) {
                register.Remove(typeof(TSubject));
            } else {
                register[typeof(TSubject)] = result;
            }
        }

        /// <summary>
        /// 버튼에 콜백을 구독한다.
        /// </summary>
        /// <param name="onNotify">콜백</param>
        /// <param name="checkObj">파괴되면 자동으로 구독해지 할 객체</param>
        public static void ListenForButton(OnNotify onNotify, GameObject checkObj = null) {
            if (checkObj != null) {
                onNotifyForButton += CheckObject;
            }
            onNotifyForButton += onNotify;

            void CheckObject() {
                if (checkObj == null) {
                    UnlistenForButton(onNotify);
                    onNotifyForButton -= CheckObject;
                }
            }
        }


        public static void UnlistenForButton(OnNotify target) {
            onNotifyForButton -= target;
        }
    }
}