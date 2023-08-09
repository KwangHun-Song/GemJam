using System.Collections.Generic;
using System.Linq;

namespace GemMatch.UndoSystem {
    /// <summary>
    /// 어떤 행동을 할 때 Undo를 함께 등록할 수 있는데, 이것을 스택에 저장해두었다가 언두를 사용 가능하게 해 주는 스크립트.
    /// </summary>
    public class UndoHandler {
        private Stack<ICommand> DoStack { get; } = new Stack<ICommand>();
        private Stack<ICommand> UndoStack { get; } = new Stack<ICommand>();

        public bool IsEmpty() {
            return DoStack.Any() == false;
        }

        public void Reset() {
            DoStack.Clear();
            UndoStack.Clear();
        }

        public void Do(ICommand command) {
            DoStack.Push(command);
            command.Redo();
        }
        
        public void Undo() {
            if (DoStack.Any() == false) return;
            
            var command = DoStack.Pop();
            
            if (command != null) {
                command.Undo();
                UndoStack.Push(command);
                
                if (command.TriggeredFromPrev) Undo();
            }
        }
        
        public void Redo() {
            if (UndoStack.Any() == false) return;
            
            var command = UndoStack.Pop();

            if (command != null) {
                command.Redo();
                DoStack.Push(command);
            }
        }
    }
}