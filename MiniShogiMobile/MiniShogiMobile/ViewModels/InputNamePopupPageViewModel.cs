using MiniShogiMobile.Conditions;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.NavigationEx;
using Prism.Services;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MiniShogiMobile.ViewModels
{
    public class InputNamePopupPageViewModel : NavigationViewModel<InputNameCondition, string>
    {
        public ObservableCollection<string> NameList { get; private set; } = new ObservableCollection<string>();
        public ReactiveProperty<string> Name { get; private set; } = new ReactiveProperty<string>();
        public ReactiveProperty<string> OkButtonText { get; private set; } = new ReactiveProperty<string>();

        private Func<string, string> NameErrorChecker;
        private Func<string, string> NameConfirmer;

        public AsyncReactiveCommand SelectCommand { get; }
        public AsyncReactiveCommand OkCommand { get; }
        public AsyncReactiveCommand CancelCommand { get; }

        public InputNamePopupPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {

            OkCommand = new AsyncReactiveCommand();
            OkCommand.Subscribe(async () =>
            {
                var error = NameErrorChecker(Name.Value);
                if(error != null)
                {
                    await pageDialogService.DisplayAlertAsync("確認", error, "OK");
                    return;
                }

                var comfirmation = NameConfirmer(Name.Value);
                if(comfirmation != null)
                {
                    if (!await pageDialogService.DisplayAlertAsync("確認", comfirmation, "はい", "いいえ"))
                        return;
                }
                await GoBackAsync(Name.Value);
            }).AddTo(this.Disposable);
            CancelCommand = new AsyncReactiveCommand();
            CancelCommand.Subscribe(async () =>
            {
                await GoBackAsync();
            }).AddTo(this.Disposable);
            SelectCommand = new AsyncReactiveCommand();
            SelectCommand.Subscribe(async () =>
            {
                var cancelText = "Cancel";
                var selected = await pageDialogService.DisplayActionSheetAsync("選択してください",cancelText, null, NameList.ToArray());
                if (selected != cancelText)
                    Name.Value = selected;

            }).AddTo(this.Disposable);
        }

        public override void Prepare(InputNameCondition parameter)
        {
            if(parameter.NameList != null)
                parameter.NameList.ForEach((x) => NameList.Add(x));
            Name.Value = parameter.DefaultName;
            OkButtonText.Value = parameter.OkButtonText;
            NameConfirmer = parameter.NameConfirmer;
            NameErrorChecker = parameter.NameErrorChecker;
        }
    }
}
