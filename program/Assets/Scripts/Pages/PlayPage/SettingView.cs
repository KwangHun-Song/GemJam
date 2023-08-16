using Cysharp.Threading.Tasks;
using DG.Tweening;
using PagePopupSystem;
using Popups;
using Record;
using ToastMessageSystem;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace Pages {
    public enum SettingViewResult { Exit, QuitGame }
    public class SettingView : MonoBehaviour {
        [SerializeField] private Image curtain;
        [SerializeField] private Transform animRoot;
        [SerializeField] private Toggle vibrationToggle;
        [SerializeField] private Toggle sfxToggle;
        [SerializeField] private Toggle bgmToggle;

        private UniTaskCompletionSource<SettingViewResult> completionSource;

        public async UniTask<SettingViewResult> ShowAsync() {
            gameObject.SetActive(true);
            
            // 먼저 값을 적용해준다.
            sfxToggle.isOn = PlayerInfo.EnableSfx;
            sfxToggle.GetComponent<ToggleAlphaActivator>().ShowEffectWithoutAnim(PlayerInfo.EnableSfx);
            
            bgmToggle.isOn = PlayerInfo.EnableBgm;
            bgmToggle.GetComponent<ToggleAlphaActivator>().ShowEffectWithoutAnim(PlayerInfo.EnableBgm);
            
            // 값 적용 후 리스너를 추가해준다.
            vibrationToggle.onValueChanged.AddListener(VibrationToggleValueChanged);
            sfxToggle.onValueChanged.AddListener(SfxToggleValueChanged);
            bgmToggle.onValueChanged.AddListener(BgmToggleValueChanged);
            
            curtain.color = Color.clear;
            curtain.DOColor(new Color(0, 0, 0, 0.6F), 0.3F);
            animRoot.localScale = Vector3.zero;
            animRoot.DOScale(Vector3.one, 0.3F).SetEase(Ease.OutQuad);

            completionSource = new UniTaskCompletionSource<SettingViewResult>();

            return await completionSource.Task;
        }

        private async UniTask HideAsync() {
            curtain.DOColor(Color.clear, 0.15F);
            animRoot.DOScale(Vector3.zero, 0.15F).SetEase(Ease.OutQuad);
            
            // 추가했던 리스너를 제거해준다.
            vibrationToggle.onValueChanged.RemoveListener(VibrationToggleValueChanged);
            sfxToggle.onValueChanged.RemoveListener(SfxToggleValueChanged);
            bgmToggle.onValueChanged.RemoveListener(BgmToggleValueChanged);
            
            gameObject.SetActive(false);
        }

        public void HideImmediately() {
            gameObject.SetActive(false);
        }

        public void OnClickSetting() {
            SimpleSound.Play(SoundName.button_click);
            HideAsync().Forget();
            completionSource?.TrySetResult(SettingViewResult.Exit);
        }

        public void OnClickQuit() {
            ClickQuitAsync().Forget();

            async UniTask ClickQuitAsync() {
                var isQuit = await PopupManager.ShowAsync<bool>(nameof(ExitPopup));
                if (isQuit) {
                    HideAsync().Forget();
                    completionSource?.TrySetResult(SettingViewResult.QuitGame);
                }
            }
        }

        private void VibrationToggleValueChanged(bool isOn) {
            SimpleSound.Play(SoundName.a_ha);
            ToastMessage.Show("It will be implemented soon.");
        }

        private void SfxToggleValueChanged(bool isOn) {
            SimpleSound.Play(SoundName.button_click);
            PlayerInfo.EnableSfx = isOn;
        }

        private void BgmToggleValueChanged(bool isOn) {
            SimpleSound.Play(SoundName.button_click);
            PlayerInfo.EnableBgm = isOn;
            
        }
    }
}