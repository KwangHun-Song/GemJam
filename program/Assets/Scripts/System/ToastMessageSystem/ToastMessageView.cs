using System.Collections;
using TMPro;
using UnityEngine;

namespace ToastMessageSystem {
    public class ToastMessageView : MonoBehaviour {
        [SerializeField] private Animator animator;
        [SerializeField] TMP_Text txtField;

        public void Text(string msg) => txtField.text = msg;

        private IEnumerator Start() {
            animator.SetTrigger("on");
            yield return new WaitForSeconds(2);
            animator.SetTrigger("off");
            yield return new WaitForSeconds(0.5f);
            Destroy(gameObject);
        }
    }
}