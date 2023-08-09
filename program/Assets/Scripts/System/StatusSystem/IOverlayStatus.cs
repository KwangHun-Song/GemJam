namespace OverlayStatusSystem {
    public interface IOverlayStatus {
        internal void Save();
        IOverlayStatusEvent EventListener { get; }
        void Enqueue(OverlayStatusParam inputParam);
    }
}