using Cysharp.Threading.Tasks;

namespace GemMatch {
    /// <summary>
    /// 활성화 여부를 받는지 여부
    /// </summary>
    public interface IReceiveActivation {
        UniTask OnActive(bool isActive);
    }
}