using UnityEngine;

namespace Record {
    /// <summary>
    /// 인벤토리 아이템만 Enum으로 사용해주세요. 샵아이템은 따로.
    /// </summary>
    public enum Item { Coin, ReadyRocket, ReadyExtraSlot, PlayUndo, PlayMagnet, PlayShuffle }
    
    public class Wallet {
        public static int GetItemCount(Item item) => PlayerPrefs.GetInt(item.ToString(), 0);
        public static void SetItemCount(Item item, int count) => PlayerPrefs.SetInt(item.ToString(), count);

        public static void Gain(Item item, int count = 1) {
            var resultCount = Mathf.Clamp(GetItemCount(item) + count, 0, int.MaxValue);
            SetItemCount(item, resultCount);
        }

        public static bool Use(Item item, int count = 1) {
            var originCount = GetItemCount(item);
            if (originCount < count) return false;
            
            SetItemCount(item, originCount - count);
            return true;
        }
    }
}