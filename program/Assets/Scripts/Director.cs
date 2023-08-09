using PagePopupSystem;
using UnityEngine;

/// <summary>
/// 가장 먼저 실행되는 클래스
/// </summary>
public class Director : MonoBehaviour {
    private void Start() {
        PageManager.ChangeImmediately(Page.MainPage);
    }
}
