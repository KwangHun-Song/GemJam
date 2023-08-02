namespace GemMatch.LevelEditor {
    public interface IEditViewEventListener {
        event System.Action<IEditViewEventListener> OnTouch;
        Tile[] Tiles { get; }
        void Input(int index);
        void ChangeTile(Tile tile);
    }
}