using System.Collections.Generic;
using System.Linq;

namespace GemMatch {
    public class Solver {
        private ISolverAI SolverAI { get; }
        
        public Solver(ISolverAI solverAI) {
            SolverAI = solverAI;
        }
        
        public SolverResult Solve(Level level) {
            var clonedCtrl = new SimulationController();
            clonedCtrl.StartGame(level);

            var clickedTileIndices = new List<int>();
            var simulationResult = SimulationResult.OnProgress;

            var safeStop = 0;
            while (simulationResult == SimulationResult.OnProgress && safeStop < 5000) {
                safeStop++;
                var tileIndex = SolverAI.GetIndexToInput(clonedCtrl);
                simulationResult = clonedCtrl.SimulationInput(tileIndex);
                clickedTileIndices.Add(tileIndex);
                
                // UnityEngine.Debug.Log($"click {tileIndex}, " +
                //                       $"memory {string.Join(",", clonedCtrl.Memory.Select(e => e.Color.ToString().Substring(0, 1)))}, " +
                //                       $"remain {string.Join(",", clonedCtrl.Tiles.SelectMany(t => t.Entities).Count())}");

                if (safeStop >= 5000) {
                    UnityEngine.Debug.Log("solver something wrong. safe stop!");
                }
            }

            var gameResult = simulationResult switch {
                SimulationResult.OnProgress => GameResult.Error,
                SimulationResult.Error => GameResult.Error,
                SimulationResult.Clear => GameResult.Clear,
                SimulationResult.Fail => GameResult.Fail,
            };

            return new SolverResult {
                gameResult = gameResult,
                tileIndices = clickedTileIndices
            };
        }
    }
}