using System.Collections.Generic;

namespace GemMatch {
    public interface IColorCalculator {
        Queue<ColorIndex> GenerateColorQueue(int queueCount, List<ColorIndex> colors);
    }
}