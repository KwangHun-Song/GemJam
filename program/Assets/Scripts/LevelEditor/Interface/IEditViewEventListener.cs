namespace GemMatch.LevelEditor {
    public interface IEditViewEventListener {
        Tile[] Tiles { get; }
        void Input(int index);
        void ChangeTile(Tile tile);
    }
}