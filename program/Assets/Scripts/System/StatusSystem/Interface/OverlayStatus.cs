using System;
using System.Collections.Generic;

namespace OverlayStatusSystem {
    /// <summary>
    /// IOverlayStatus의 추상화 클래스
    /// </summary>
    /// <typeparam name="ParamType">Save이후 불리는 OnEvent의 매개변수형</typeparam>
    public abstract class OverlayStatus<ParamType> : IOverlayStatus {
        public IOverlayStatusEvent EventListener { get; private set; }
        public Queue<OverlayStatusParam> EventRecord { get; private set; } = new Queue<OverlayStatusParam>();
        protected Action<ParamType> OnEvent;

        public OverlayStatus(IOverlayStatusEvent statusView, Action<ParamType> onEvent) {
            EventListener = statusView;
            this.OnEvent += onEvent;
        }

        public virtual void Save() {
            while (EventRecord.Count > 0) {
                var param = (ParamType)EventRecord.Dequeue().Value;
                OnEvent?.Invoke(param);
            }
        }

        public void Enqueue(OverlayStatusParam inputParam) {
            EventRecord.Enqueue(inputParam);
        }
    }
}