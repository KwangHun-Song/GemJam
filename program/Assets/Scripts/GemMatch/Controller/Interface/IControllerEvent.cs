namespace GemMatch {
    public interface IControllerEvent {
        void OnStartGame(Controller controller);
        void OnClearGame(Mission[] missions);
        void OnFailGame(Mission[] missions);
        void OnReplayGame(Mission[] missions);
        
        void OnMoveToMemory(Controller controller, Tile tile, Entity entity);
        void OnRemoveMemory(Controller controller,Entity entity);
    }
}