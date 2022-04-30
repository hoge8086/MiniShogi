using MiniShogiMobile.Conditions;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.NavigationEx;
using Prism.Services;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MiniShogiMobile.ViewModels
{
    public class SelectKomaPageViewModel : NavigationViewModel<SelectKomaConditions, string>
    {
        public AsyncReactiveCommand OkCommand { get; }
        public AsyncReactiveCommand CancelCommand { get; }
        public ObservableCollection<string> KomaNameList { get; }
        public ReactiveProperty<string> SelectedKomaName { get; }

        public SelectKomaPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            KomaNameList = new ObservableCollection<string>();
            SelectedKomaName = new ReactiveProperty<string>();

            OkCommand = new AsyncReactiveCommand();
            OkCommand.Subscribe(async () =>
            {
                await GoBackAsync(SelectedKomaName.Value);

            }).AddTo(this.Disposable);
            CancelCommand = new AsyncReactiveCommand();
            CancelCommand.Subscribe(async () =>
            {
                await GoBackAsync();
            }).AddTo(this.Disposable);
        }

        public override void Prepare(SelectKomaConditions parameter)
        {
            foreach(var name in parameter.KomaNameList)
                KomaNameList.Add(name);
            if (parameter.SelectedKoma == null)
                SelectedKomaName.Value = KomaNameList.FirstOrDefault();
            else
                SelectedKomaName.Value = parameter.SelectedKoma;
        }
    }
}
