using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using Object = UnityEngine.Object;

namespace PagePopupSystem {
    public class PopupManager : MonoBehaviour {
        private static Dictionary<string, PopupHandler> popups;

        private static Dictionary<string, PopupHandler> Popups {
            get {
                if (popups != null) return popups;
                popups = new Dictionary<string, PopupHandler>();

                foreach (var popup in FindObjectsOfType<PopupHandler>(true)) {
                    if (popups.ContainsKey(popup.Name)) {
                        Debug.LogError($"Duplicate popup name found: {popup.Name}. Please ensure each popup has a unique name.");
                    } else {
                        popups.Add(popup.Name, popup);
                    }
                }

                return popups;
            }
        }

        private static readonly Stack<PopupHandler> popupStack = new Stack<PopupHandler>();

        public static PopupHandler CurrentPopup {
            get {
                if (!popupStack.Any()) {
                    throw new InvalidOperationException("No popups in the stack.");
                }

                return popupStack.Peek();
            }
        }

        public static async UniTask<T> ShowAsync<T>(string popupName, object param = null) {
            if (!Popups.ContainsKey(popupName)) {
                throw new ArgumentException($"Popup with name {popupName} not found.");
            }

            var popup = Popups[popupName];
            
            if (popupStack.Any()) {
                popup.SetSortingOrder(CurrentPopup.GetSortingOrder() + 1000);
            }

            popupStack.Push(popup);
            
            await popup.ShowWithAnimation(param);

            var result = await popup.popupTask.Task;
            
            if (popupStack.Count > 0 && popupStack.Peek() == popup) {
                popupStack.Pop();
            } else {
                Debug.LogWarning($"Unexpected popup state. {popupName} is not on top of the stack.");
            }

            if (result is T convertedResult) {
                return convertedResult;
            } else {
                return default;
            }
        }
    }
}
