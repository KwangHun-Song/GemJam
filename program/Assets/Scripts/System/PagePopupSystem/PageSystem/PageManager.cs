using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Linq;

namespace PagePopupSystem {
    public class PageManager {
        private static Dictionary<string, PageHandler> pages;

        private static Dictionary<string, PageHandler> Pages => pages ??= Object
            .FindObjectsOfType<PageHandler>(true)
            .ToDictionary(page => page.Name);
        
        internal static async UniTaskVoid ChangeTo(string pageName, object param, GameObject currenPage) {
            await FadeOutHelper.FadeOut();
            currenPage.SetActive(false);
            var nextPage = Pages[pageName];
            nextPage.gameObject.SetActive(true);
            nextPage.OnWillEnter(param);
            await FadeOutHelper.FadeIn();
        }
    }
}