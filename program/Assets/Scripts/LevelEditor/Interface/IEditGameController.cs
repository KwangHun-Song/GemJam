namespace GemMatch.LevelEditor {
    public interface IEditGameController : IEditViewEventListener, IEditToolEventListener, IEditInspectorEventListener{
        void Initialize(string levelStream); // level stream으로 데이터를 채운다
        void LoadInspector();
        void Input(int tileIndex);
    }
}