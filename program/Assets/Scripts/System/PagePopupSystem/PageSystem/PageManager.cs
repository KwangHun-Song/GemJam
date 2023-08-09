using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Linq;

namespace PagePopupSystem {
    public enum Page { None, PlayPage, MainPage }
    public class PageManager {
        private static Dictionary<Page, PageHandler> pages;
        public static Page CurrentPage { get; private set; }

        private static Dictionary<Page, PageHandler> Pages => pages ??= Object
            .FindObjectsOfType<PageHandler>(true)
            .ToDictionary(page => page.GetPageType());

        public static void ChangeImmediately(Page pageType, object param = null) {
            var nextPage = Pages[pageType];
            nextPage.gameObject.SetActive(true);
            nextPage.OnWillEnter(param);
            CurrentPage = pageType;
        }
        
        public static async UniTaskVoid ChangeTo(Page pageType, object param = null) {
            if (pageType == Page.None) return;
            
            if (CurrentPage != Page.None) {
                await FadeOutHelper.FadeOut();
                pages[CurrentPage].gameObject.SetActive(false);
            }
            
            var nextPage = Pages[pageType];
            nextPage.gameObject.SetActive(true);
            nextPage.OnWillEnter(param);
            CurrentPage = pageType;
            await FadeOutHelper.FadeIn();
        }
    }
}