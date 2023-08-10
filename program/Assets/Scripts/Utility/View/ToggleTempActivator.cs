using UnityEngine;
using UnityEngine.UI;

namespace Utility {
    /// <summary>
    /// 임시로 만들었으며, 퍼포먼스가 좋지 않으므로 리소스를 적용한 후에 제거하기
    /// </summary>
    [RequireComponent(typeof(Toggle))]
    public class ToggleTempActivator : MonoBehaviour {
        [SerializeField] private GameObject selectObject;
        
        private Toggle toggle;
        private Toggle Toggle => toggle ??= GetComponent<Toggle>();

        private void Update() {
            if (Toggle.isOn) {
                if (selectObject.activeSelf == false) selectObject.SetActive(true);
            } else {
                if (selectObject.activeSelf) selectObject.SetActive(false);
            }
        }
    }
}