using System.Collections.Generic;

namespace GemMatch {
    public interface IAbility {
        AbilityIndex Index { get; }
        void Run();
        void Undo();
        IEnumerable<IAbility> GetCascadedAbility();
    }
}