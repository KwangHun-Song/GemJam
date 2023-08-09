namespace OverlayStatusSystem {
    public interface IOverlayStatus {
        void Save();
        IOverlayStatusEvent EventListener { get; }
        void Enqueue(OverlayStatusParam inputParam);
    }
}