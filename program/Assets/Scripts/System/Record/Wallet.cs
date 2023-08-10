using System;
using UnityEngine;

namespace Record {
    /// <summary>
    /// 인벤토리 아이템만 Enum으로 사용해주세요. 샵아이템은 따로.
    /// </summary>
    public enum Item { Coin, ReadyRocket, ReadyExtraSlot, PlayUndo, PlayMagnet, PlayShuffle }
    
    public class Wallet {
        public static int GetItemCount(Item item) => PlayerPrefs.GetInt(item.ToString(), 0);
        public static void SetItemCount(Item item, int count) => PlayerPrefs.SetInt(item.ToString(), count);
        public static int GetItemPrice(Item item) => WalletPriceList.GetPrice(item).price;

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

    public record WalletPrice {
        public Item item;
        public int price;

        public WalletPrice(Item item, int price)
        => (this.item, this.price) = (item, price);
    }

    public static class WalletPriceList {
        public static WalletPrice GetPrice(Item item) => item switch {
            Item.ReadyRocket => new WalletPrice(Item.ReadyRocket, 40),
            Item.ReadyExtraSlot => new WalletPrice(Item.ReadyExtraSlot, 50),
            Item.PlayUndo => new WalletPrice(Item.PlayUndo, 30),
            Item.PlayMagnet => new WalletPrice(Item.PlayMagnet, 30),
            Item.PlayShuffle => new WalletPrice(Item.PlayShuffle, 20),
            _ => throw new ArgumentOutOfRangeException(nameof(item), item, null)
        };
    }
}