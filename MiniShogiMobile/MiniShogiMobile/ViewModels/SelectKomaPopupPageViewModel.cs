using MiniShogiMobile.Conditions;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.NavigationEx;
using Prism.Services;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Rg.Plugins.Popup.Pages;
using Shogi.Business.Domain.Model.Komas;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MiniShogiMobile.ViewModels
{
    public class SelectKomaPopupPageViewModel : NavigationViewModel<SelectKomaConditions, KomaTypeId>
    {
        public AsyncReactiveCommand OkCommand { get; }
        public AsyncReactiveCommand CancelCommand { get; }
        public ObservableCollection<KomaTypeId> KomaTypeIdList { get; }
        public ReactiveProperty<KomaTypeId> SelectedKomaTypeId { get; }

        public SelectKomaPopupPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            Title = "駒を選択してください";
            var komaList = App.CreateGameService.KomaTypeRepository.FindAll().ToDictionary(x => x.Id);
            KomaTypeIdList = new ObservableCollection<KomaTypeId>(komaList.Keys);
            SelectedKomaTypeId = new ReactiveProperty<KomaTypeId>();

            OkCommand = new AsyncReactiveCommand();
            OkCommand.Subscribe(async () =>
            {
                if(SelectedKomaTypeId.Value == null)
                    await GoBackAsync();

                await GoBackAsync(SelectedKomaTypeId.Value);

            }).AddTo(this.Disposable);
            CancelCommand = new AsyncReactiveCommand();
            CancelCommand.Subscribe(async () =>
            {
                await GoBackAsync();
            }).AddTo(this.Disposable);
        }

        public override void Prepare(SelectKomaConditions parameter)
        {
            // FIX:Xamarinの不具合(CollectionViewの初期選択が反映されない)
            // <https://stackoverflow-com.translate.goog/questions/75593079/programmatically-setting-the-selecteditem-of-a-collectionview-is-not-working-on?_x_tr_sl=en&_x_tr_tl=ja&_x_tr_hl=ja&_x_tr_pto=sc>
            if (parameter.SelectedKoma == null)
                SelectedKomaTypeId.Value = KomaTypeIdList.FirstOrDefault();
            else
                SelectedKomaTypeId.Value = parameter.SelectedKoma;

            if (parameter.Title != null)
                Title = parameter.Title;
        }
    }
}
