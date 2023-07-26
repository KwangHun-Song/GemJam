namespace GemMatch {
    public interface IControllerEvent {
        void OnStartGame(Tile[] tiles, Mission[] missions);
        void OnClearGame(Mission[] missions);
        void OnFailGame(Mission[] missions);
        void OnReplayGame(Mission[] missions);
        
        void OnAddMemory(Entity entity);
        void OnRemoveMemory(Entity entity);
    }
}