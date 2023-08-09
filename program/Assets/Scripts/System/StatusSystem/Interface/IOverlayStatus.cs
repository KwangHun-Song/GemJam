namespace OverlayStatusSystem {
    /// <summary>
    /// OverlayStatusManager가 다루는 데이터 클래스
    /// </summary>
    public interface IOverlayStatus {
        void Save();
        IOverlayStatusEvent EventListener { get; }
        void Enqueue(OverlayStatusParam inputParam);
    }
}