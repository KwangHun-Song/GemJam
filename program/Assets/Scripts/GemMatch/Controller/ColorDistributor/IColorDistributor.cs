namespace GemMatch {
    public interface IColorDistributor {
        public void DistributeClearableColors(Level level, Tile[] tiles, IColorCalculator colorCalculator);
    }
}