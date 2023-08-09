using Cysharp.Threading.Tasks;

namespace GemMatch {
    public interface IAbilityView {
        UniTask RunAbilityAsync(View view, IAbility ability, Controller controller);
        UniTask RestoreAbilityAsync(View view, IAbility ability, Controller controller);
    }
}