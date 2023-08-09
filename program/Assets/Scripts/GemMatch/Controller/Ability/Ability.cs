using System.Collections.Generic;
using GemMatch.UndoSystem;

namespace GemMatch {
    public abstract class Ability<TUndo> : IAbility {
        public abstract AbilityIndex Index { get; }
        public virtual Tile TargetTile { get; }
        public Controller Controller { get; }

        public Command<TUndo> Command { get; private set; }

        public Ability(Tile targetTile, Controller controller) {
            TargetTile = targetTile;
            Controller = controller;
        }
        
        public virtual TUndo UndoParam { get; protected set; }
        
        public abstract void Run();
        public abstract void Undo(TUndo undoParam);
        
        public void Undo() => Undo(UndoParam);

        public virtual IEnumerable<IAbility> GetCascadedAbility() {
            yield break;
        }
    }
}