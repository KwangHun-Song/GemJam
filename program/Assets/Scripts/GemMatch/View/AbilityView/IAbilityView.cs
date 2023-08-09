using Cysharp.Threading.Tasks;

namespace GemMatch {
    public interface IAbilityView {
        UniTask RunAbilityAsync(View view, Ability ability, Controller controller);
    }
}