namespace GemMatch {
    public interface ITileEvent {
        void OnInitialize(Tile tile);
        void OnAddEntity(Entity entity);
        void OnRemoveLayer(Layer layer);
    }
}