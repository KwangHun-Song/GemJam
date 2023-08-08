namespace GemMatch {
    public interface IColorDistributor {
        IColorCalculator GetColorCalculator();
        bool DistributeColors(Level level, Tile[] tiles);
    }
}