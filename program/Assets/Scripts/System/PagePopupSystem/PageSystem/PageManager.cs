using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using Utility;
using Object = UnityEngine.Object;

namespace PagePopupSystem {
    public enum Page { None, PlayPage, MainPage, EditPage }
    public class PageManager {
        private static Dictionary<Page, PageHandler> pages;
        public static Page CurrentPage { get; private set; }
        public static event Action<Page> OnPageChanged;
        
        public static bool OnTransitionAnimation { get; private set; }

        private static Dictionary<Page, PageHandler> Pages => pages ??= Object
            .FindObjectsOfType<PageHandler>(true)
            .ToDictionary(page => page.GetPageType());

        public static void ChangeImmediately(Page pageType, object param = null) {
            if (CurrentPage != Page.None) Pages[CurrentPage].gameObject.SetActive(false);
            var nextPage = Pages[pageType];
            nextPage.gameObject.SetActive(true);
            nextPage.OnWillEnter(param);
            SimpleSound.PlayBGM(nextPage.BgmName);
            CurrentPage = pageType;
        }
        
        public static async UniTaskVoid ChangeTo(Page pageType, object param = null) {
            if (pageType == Page.None) return;
            
            if (CurrentPage != Page.None) {
                OnTransitionAnimation = true;
                SimpleSound.StopBGM(0.5F);
                await FadeOutHelper.FadeOut();
                Pages[CurrentPage].gameObject.SetActive(false);
            }
            
            var nextPage = Pages[pageType];
            nextPage.gameObject.SetActive(true);
            nextPage.OnWillEnter(param);
            CurrentPage = pageType;

            await UniTask.DelayFrame(1);
            OnPageChanged?.Invoke(CurrentPage);

            SimpleSound.PlayBGM(nextPage.BgmName);
            await FadeOutHelper.FadeIn();
            OnTransitionAnimation = false;
            nextPage.OnDidEnter(param);
        }

        public static void RemovePage(Page pageType) => Pages.Remove(pageType);

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void InitializePages() {
            foreach (var kvp in Pages) {
                kvp.Value.gameObject.SetActive(false);
            }

            SceneManager.activeSceneChanged += (_, _) => { pages = null; };
        }
    }
}