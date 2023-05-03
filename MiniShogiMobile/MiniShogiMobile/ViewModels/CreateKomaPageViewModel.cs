using Prism.Commands;
using System;
using Prism.Navigation;
using Prism.NavigationEx;
using Prism.Services;
using System.Collections.ObjectModel;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Shogi.Business.Domain.Model.Komas;
using MiniShogiMobile.Utils;

namespace MiniShogiMobile.ViewModels
{

    // MEMO:NavigationViewModelのResultは本来いらないが、Resultを指定しないと呼び出し元で待機しないので、仕方なくつける(要NavigationViewModelの改善)
    public class CreateKomaPageViewModel : NavigationViewModel<KomaTypeId, bool>
    {
        public AsyncReactiveCommand<CellForCreateKomaViewModel> ChangeMoveCommand { get; set; }
        public AsyncReactiveCommand SaveCommand { get; set; }
        public KomaInfoViewModel KomaInfo { get; set; }

        private KomaType OldKomaType;

        private readonly int BoardSize = 9;
        public CreateKomaPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            KomaInfo = new KomaInfoViewModel();

            ChangeMoveCommand = new AsyncReactiveCommand<CellForCreateKomaViewModel>();
            ChangeMoveCommand.Subscribe(async x =>
            {
                if (x.Position == KomaInfo.KomaPosition)
                    return;

                x.MoveType.Value = (MoveType)(((int)x.MoveType.Value + 1) % (int)(MoveType.RepeatableJump + 1));

                // MEMO:不成と成りでコマンドを分けた方がよいがResourcesでControlTemplateを使ってるのでコマンドを分けれない
                //      なので、両方の盤を更新する(他に何か用方法があれば)
                KomaInfo.UpdateCanMoveCellByRepeatableJump(KomaInfo.Board);
                KomaInfo.UpdateCanMoveCellByRepeatableJump(KomaInfo.PromotedBoard);
            });
            SaveCommand = new AsyncReactiveCommand();
            SaveCommand.Subscribe(async x =>
            {
                await this.CatchErrorWithMessageAsync(async () =>
                {
                    if(OldKomaType != null)
                    {
                        bool doDelete = await pageDialogService.DisplayAlertAsync("確認", "既にゲームへ配置済みの駒は変更されません。よろしいですか？", "はい", "いいえ");
                        if (!doDelete)
                            return;
                    }
                    App.CreateGameService.KomaTypeRepository.Replace(KomaInfo.CreateKomaTypeFromBoard(), OldKomaType);
                    await GoBackAsync(true);
                });
            });
        }

        public override void Prepare(KomaTypeId parameter)
        {
            KomaType komaType;
            if (parameter != null)
            {
                komaType = App.CreateGameService.KomaTypeRepository.FindById(parameter);
                OldKomaType = komaType;
            }
            else
            {
                komaType = new KomaType();
                OldKomaType = null; 
            }
            KomaInfo.Update(komaType);
        }
    }
}
