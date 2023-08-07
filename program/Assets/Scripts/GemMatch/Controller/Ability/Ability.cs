using System.Collections.Generic;

namespace GemMatch {
    public abstract class Ability {
        public virtual Tile TargetTile { get; }
        public Controller Controller { get; }
        
        public Ability(Tile targetTile, Controller controller) {
            TargetTile = targetTile;
            Controller = controller;
        }
        
        public abstract Ability Run();
        public abstract IEnumerable<Ability> GetCascador();
    }
}