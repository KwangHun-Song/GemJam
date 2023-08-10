using GemMatch.UndoSystem;

namespace GemMatch {
    public class AbilityCommand : Command {
        public AbilityCommand(Controller controller, IAbility ability, bool triggeredByPrev = false) : base(
            @do:() => {
                ability.Run();
                foreach (var listener in controller.Listeners) listener.OnRunAbility(ability);
            }, 
            undo:() => {
                ability.Undo();
                foreach (var listener in controller.Listeners) listener.OnRestoreAbility(ability);
            }, triggeredByPrev) { }
    }
}