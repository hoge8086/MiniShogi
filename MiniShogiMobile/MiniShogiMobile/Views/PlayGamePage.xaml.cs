using MiniShogiMobile.Controls;
using MiniShogiMobile.Utils;
using MiniShogiMobile.ViewModels;
using Shogi.Business.Domain.Model.Games;
using Shogi.Business.Domain.Model.PlayerTypes;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MiniShogiMobile.Views
{
    public partial class PlayGamePage : BasePage
    {
        public PlayGamePage()
        {
            InitializeComponent();
            //prism:ViewModelLocator.AutowireViewModel="True"だと既にここでBindingContextにViewModelが注入されている
            var vm = BindingContext as PlayGamePageViewModel;
            if (vm == null)
                return;

            vm.StartAnimationOfKomaMoving = AnimateKomaMovingAsync;
            vm.EndAnimationOfKomaMoving = async () =>
            {
                // MEMO:ちらつき防止のため盤のUI描画を少し待ってから処理する
                await Task.Delay(10);
                // アニメーション終了
                movingLayer.Children.Clear();
            };
        }

        /// <summary>
        /// 駒の動きのアニメーション
        /// </summary>
        /// <remarks>
        /// 色々微調整(Task.Delay)や問題があり暫定的なソリューションとする
        /// 盤(持ち駒含む)の上に配置したAbsoluteLayoutの中に移動用の駒をオーバーラップさせてアニメーションする
        /// ※元の駒自身を動かすのは表示順序がスタックレイアウト順なので、駒をアニメーションしても最前面に表示されないため、この方法を採用した
        /// </remarks>
        /// <param name="moveCommand"></param>
        /// <returns></returns>
        public async Task AnimateKomaMovingAsync(MoveCommand moveCommand)
        {
            // TODO:リファクタリング
            KomaView koma = null;
            Action deleteKoma = null;
            // 移動対象の駒を取得
            if (moveCommand is BoardKomaMoveCommand  boardMoveCommand )
            {
                var srcCell = board.GetCell(boardMoveCommand.FromPosition.X, boardMoveCommand.FromPosition.Y) as CellView;
                if (srcCell == null)
                    return;
                koma = srcCell.GetKomaView();
                deleteKoma = ()=> {
                    // MEMO:ここでViewModelをいじるのはよくないが、ViewのIsVisibleプロパティを直接いじるとBindingが外れるので仕方なくいじる.他に良い方法あれば直す
                    var sourceCellViewModel = srcCell.BindingContext as CellPlayingViewModel;
                    if (sourceCellViewModel != null)
                        sourceCellViewModel.Koma.Value = null;
                };
            }
            else if(moveCommand is HandKomaMoveCommand handKomaMoveCommand)
            {
                var playerView = handKomaMoveCommand.Player == PlayerType.Player1 ? player1View : player2View;
                var hands = playerView.GetHandKomaViews();
                var hand = hands.FirstOrDefault(x => (x.BindingContext is HandKomaViewModel vm) && (vm.KomaTypeId == handKomaMoveCommand.KomaTypeId));
                koma = hand.GetKoma();
                deleteKoma = ()=> {
                    // MEMO:ここでViewModelをいじるのはよくないが、ViewのIsVisibleプロパティを直接いじるとBindingが外れるので仕方なくいじる.他に良い方法あれば直す
                    var handKomaViewMode = hand.BindingContext as HandKomaViewModel;
                    if (handKomaViewMode != null)
                        handKomaViewMode.Num.Value -= 1;
                };
            }
            if (koma == null)
                return;

            // 移動用アニメーションの駒の外見を移動対象の駒と合わせる
            var movingKoma = new KomaView(koma);
            var srcKomaScreenCoords = koma.GetScreenCoords(field);
            // MEMO:Androidだとなぜか、IsVisibleをtureにしてからChildrenをAddしないとChildrenが表示されないので注意
            movingLayer.Children.Add(movingKoma, new Rectangle(srcKomaScreenCoords.X, srcKomaScreenCoords.Y, koma.Width, koma.Height));
            // MEMO:初手が変な位置から表示されるため、Children.Add()の描画を待つ
            await Task.Delay(100);

            // 移動対象の駒を消す(IsVisibleをいじるとVMとのIsVisibleのバインディングが消える)
            deleteKoma?.Invoke();

            // 移動先座標を取得
            var destCell = board.GetCell(moveCommand.ToPosition.X, moveCommand.ToPosition.Y) as CellView;
            var destScreenCoords = destCell.GetScreenCoords(field);

            // 移動アニメーション開始
            await movingKoma.LayoutTo(new Rectangle(destScreenCoords.X, destScreenCoords.Y, destCell.Height, destCell.Width), 320, Easing.SinOut);
        }
    }
}
