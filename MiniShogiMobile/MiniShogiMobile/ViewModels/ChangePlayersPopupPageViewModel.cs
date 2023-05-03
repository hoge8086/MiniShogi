using MiniShogiMobile.Conditions;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.NavigationEx;
using Prism.Services;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Shogi.Business.Domain.Model.PlayerTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MiniShogiMobile.ViewModels
{
    public class ChangePlayersPopupPageViewModel : NavigationViewModel<ChangePlayersCondition, ChangePlayersCondition>
    {
        public SelectPlayperViewModel Player1 { get; set; }
        public SelectPlayperViewModel Player2 { get; set; }
        public AsyncReactiveCommand OkCommand { get; }
        public AsyncReactiveCommand CancelCommand { get; }
        public ChangePlayersPopupPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            Player1 = new SelectPlayperViewModel(PlayerThinkingType.Human);
            Player2 = new SelectPlayperViewModel(PlayerThinkingType.AI, 5);
            OkCommand = new AsyncReactiveCommand();
            OkCommand.Subscribe(async () =>
            {
                await GoBackAsync(new ChangePlayersCondition(Player1.CreatePlayer(PlayerType.Player1), Player2.CreatePlayer(PlayerType.Player2)));
            }).AddTo(this.Disposable);
            CancelCommand = new AsyncReactiveCommand();
            CancelCommand.Subscribe(async () =>
            {
                await GoBackAsync();
            }).AddTo(this.Disposable);
        }
        public override void Prepare(ChangePlayersCondition parameter)
        {
            Player1.Update(parameter.Player1);
            Player2.Update(parameter.Player2);
        }
    }
}
