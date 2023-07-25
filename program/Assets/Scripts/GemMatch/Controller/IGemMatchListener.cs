namespace GemMatch {
    public interface IGemMatchListener {
        void OnStartGame(Tile[] tiles, Mission[] missions);
        void OnClearGame(Mission[] missions);
        void OnFailGame(Mission[] missions);
        void OnReplayGame(Mission[] missions);

        void OnAddMemory(ColorIndex color, int memoryIndex);
        void OnRemoveMemory(ColorIndex color);
    }
}