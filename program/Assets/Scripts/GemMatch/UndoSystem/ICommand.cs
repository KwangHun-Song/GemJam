namespace GemMatch.UndoSystem {
    public interface ICommand {
        void Do();
        void Redo();
        void Undo();
        bool TriggeredFromPrev { get; }
    }

}