using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.NavigationEx;
using Prism.Services;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Shogi.Business.Domain.Model.Komas;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MiniShogiMobile.ViewModels
{
    public class KomaInfoPopupPageViewModel : NavigationViewModel<KomaType>
    {
        public AsyncReactiveCommand OkCommand { get; }
        public KomaInfoViewModel KomaInfo { get; set; }
        public KomaInfoPopupPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            KomaInfo = new KomaInfoViewModel();
            OkCommand = new AsyncReactiveCommand();
            OkCommand.Subscribe(async () =>
            {
                await GoBackAsync();
            }).AddTo(this.Disposable);

        }

        public override void Prepare(KomaType komaType)
        {
            KomaInfo.Update(komaType);
        }
    }
}
