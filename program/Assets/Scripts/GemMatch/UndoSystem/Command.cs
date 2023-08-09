using System;

namespace GemMatch.UndoSystem {
    public class Command : ICommand {
        public readonly Action @do;
        public readonly Action undo;
        public bool TriggeredFromPrev { get; }

        public Command(Action @do, Action undo, bool triggeredByPrev = false) {
            this.@do = @do;
            this.undo = undo;
            TriggeredFromPrev = triggeredByPrev;
        }

        public void Do() => @do?.Invoke();
        public void Undo() => undo?.Invoke();
        public void Redo() => Do();
    }
    
    public class Command<T> : ICommand {
        public readonly Action @do;
        public readonly Action<T> undo;
        public readonly T param;
        public bool TriggeredFromPrev { get; }

        public Command(Action @do, Action<T> undo, T param = default, bool triggeredByPrev = false) {
            this.@do = @do;
            this.undo = undo;
            this.param = param;
            TriggeredFromPrev = triggeredByPrev;
        }

        public void Do() => @do?.Invoke();
        public void Undo() => undo?.Invoke(param);
        public void Redo() => Do();
    }
}