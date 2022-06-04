using MiniShogiMobile.Controls;
using MiniShogiMobile.Utils;
using MiniShogiMobile.ViewModels;
using Shogi.Business.Domain.Model.Games;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MiniShogiMobile.Views
{
    public partial class PlayGamePage : BasePage
    {
        public PlayGamePage()
        {
            InitializeComponent();

            var vm = BindingContext as PlayGamePageViewModel;
            if (vm == null)
                return;

            vm.AnimateKomaMoving = AnimateKomaMovingAsync;
        }

        public async Task AnimateKomaMovingAsync(MoveCommand moveCommand)
        {
            var boardMoveCommand = moveCommand as BoardKomaMoveCommand;
            if (boardMoveCommand == null)
                return;

            // 移動対象の駒を取得
            var srcCell = board.GetCell(boardMoveCommand.FromPosition.X, boardMoveCommand.FromPosition.Y) as CellView;
            if (srcCell == null)
                return;
            var koma = srcCell.GetKomaView();
            //System.Diagnostics.Debug.WriteLine($"from:({x}, {y}) size={koma.Height}");

            // 移動用アニメーション駒の外見を移動対象の駒と合わせる
            var movingKoma = new KomaView(koma);
            var srcKomaScreenCoords = koma.GetScreenCoords(field);
            movingLayer.Children.Add(movingKoma, new Rectangle(srcKomaScreenCoords.X, srcKomaScreenCoords.Y, koma.Width, koma.Height));
            movingLayer.IsVisible = true;

            // 移動対象の駒を消す(IsVisibleをいじるとVMとのIsVisibleのバインディングが消える)
            var sourceCellViewModel = srcCell.BindingContext as CellPlayingViewModel;
            if (sourceCellViewModel != null)
                sourceCellViewModel.Koma.Value = null;

            // 移動距離・方向を計算
            var destCell = board.GetCell(boardMoveCommand.ToPosition.X, boardMoveCommand.ToPosition.Y) as CellView;
            var srcScreenCoords = srcCell.GetScreenCoords(field);
            var destScreenCoords = destCell.GetScreenCoords(field);
            var dx = destScreenCoords.X - srcScreenCoords.X;
            var dy = destScreenCoords.Y - srcScreenCoords.Y;

            // 移動アニメーション開始
            //System.Diagnostics.Debug.WriteLine($"dx,dy:({dx}, {dy}) size={koma.Height}");
            await movingKoma.TranslateTo(dx, dy, 200);

            // アニメーション終了
            // FIX:ちらつきを防止したい(消した後に描画を更新して、駒が配置されるので)
            movingLayer.Children.Clear();
            movingLayer.IsVisible = false;
        }

    }
}
