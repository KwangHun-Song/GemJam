using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using OverlayStatusSystem;
using UnityEngine;
using Utility;

namespace GemMatch {
    public class ClearCoinAnimator {
        public async UniTask ShowCoinAnimation(TileView[] tileViews, Transform coinTarget) {
            // 빈 타일뷰들 선정. 랜덤 순서로 만들어줄 거라고 한 번 섞어준다.
            var targetTileViews = tileViews
                .Where(tv => tv.Tile.Entities.Any() == false && tv.Tile.IsOpened)
                .Shuffle().ToArray();

            var coinGo = Resources.Load<CoinBonus>(nameof(CoinBonus));
            var coins = new List<CoinBonus>();
            
            // 코인뷰들 Instantiate. 0.015초 간격으로 랜덤으로 생성한다.
            foreach (var tileView in targetTileViews) {
                var coinView = Object.Instantiate(coinGo, tileView.transform);
                coinView.transform.localScale = Vector3.one * 0.66F;
                coinView.ShowStart();
                coins.Add(coinView);
                await UniTask.Delay(15);
            }

            // 순서를 한 번 섞어주고, Crash 이펙트 후 타겟으로 이동시킨다.
            coins = coins.Shuffle().ToList();
            await UniTask.WhenAll(coins.Select(coinView => ShowCrashAndMoveAsync(coinView, coinTarget)));
        }

        private async UniTask ShowCrashAndMoveAsync(CoinBonus coinView, Transform coinTarget) {
            // 출발을 랜덤으로 하도록 랜덤 딜레이를 추가
            await UniTask.Delay(Random.Range(400, 1200));
            // 크래시 이펙트
            coinView.ShowCrashAsync().Forget();

            // 코인 하나당 1월씩 획득
            OverlayStatusHelper.CollectCoin(1);
            // 날리기
            coinView.transform.SetParent(OverlayStatusHelper.GetCoinStatusRoot());
            await coinView.transform.DOLocalMove(Vector3.zero, 0.8F).SetEase(Ease.InBack, 2.5F).ToUniTask();
            
            OverlayStatusHelper.UpdateCoinStatus();
            SimpleSound.Play(SoundName.coin_get);
            Object.DestroyImmediate(coinView.gameObject);
        }
    }
}