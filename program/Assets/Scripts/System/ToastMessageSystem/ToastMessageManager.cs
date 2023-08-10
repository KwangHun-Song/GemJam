using System;
using UnityEngine;

namespace ToastMessageSystem {
    public class ToastMessage : MonoBehaviour {
        private static ToastMessage instance = null;
        public static ToastMessage Instance {
            get {
                if (null == instance) {
                    var obj = FindObjectOfType<ToastMessage>();
                    if (null != obj) {
                        instance = obj;
                        return instance;
                    }
                    var newObj = new GameObject();
                    instance = newObj.AddComponent<ToastMessage>();
                    newObj.name = "[Singleton]ToastMessage";
                    DontDestroyOnLoad(newObj);
                    return instance;
                }
                return instance;
            }
        }

        private GameObject toastPrefabCache = null;

        public static void Show(string message) {
            if (null == Instance.toastPrefabCache) {
                Instance.toastPrefabCache = Resources.Load<GameObject>("ToastMessage");
            }
            var toast = Instantiate(Instance.toastPrefabCache, null);
            toast.GetComponent<ToastMessageView>().Text(message);
        }
    }
}