using MiniShogiMobile.Conditions;
using MiniShogiMobile.Utils;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.NavigationEx;
using Prism.Services;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Shogi.Business.Domain.Model.GameTemplates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiniShogiMobile.ViewModels
{
    public class GameRulePopupPageViewModel : NavigationViewModel<GameTemplate>
    {
        public AsyncReactiveCommand OkCommand { get; }
        public ReactiveProperty<int> TerritoryBoundary { get; }
        public ReactiveProperty<WinConditionType> WinCondition { get; }
        public ReactiveProperty<bool> EnableNiHu { get; }
        public ReactiveProperty<bool> EnableCheckmateByHandHu { get; }
        public ReactiveProperty<bool> EnableKomaCannotMove { get; }
        public ReactiveProperty<bool> EnableLeaveOte { get; }
        GameTemplate GameTemplate;

        public GameRulePopupPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            TerritoryBoundary = new ReactiveProperty<int>();
            WinCondition = new ReactiveProperty<WinConditionType>();
            EnableNiHu = new ReactiveProperty<bool>();
            EnableCheckmateByHandHu = new ReactiveProperty<bool>();
            EnableKomaCannotMove = new ReactiveProperty<bool>();
            EnableLeaveOte = new ReactiveProperty<bool>();
            
            OkCommand = new AsyncReactiveCommand();
            OkCommand.Subscribe(async () =>
            {
                GameTemplate.TerritoryBoundary = TerritoryBoundary.Value;
                GameTemplate.WinCondition = WinCondition.Value;
                GameTemplate.ProhibitedMoves = new ProhibitedMoves(
                    EnableNiHu.Value,
                    EnableCheckmateByHandHu.Value,
                    EnableKomaCannotMove.Value,
                    EnableLeaveOte.Value
                    );
                await GoBackAsync();
            }).AddTo(this.Disposable);
        }

        public override void Prepare(GameTemplate parameter)
        {
            GameTemplate = parameter.Clone();
            TerritoryBoundary.Value = parameter.TerritoryBoundary;
            WinCondition.Value = parameter.WinCondition;
            EnableNiHu.Value = parameter.ProhibitedMoves.EnableNiHu;
            EnableKomaCannotMove.Value = parameter.ProhibitedMoves.EnableKomaCannotMove;
            EnableCheckmateByHandHu.Value = parameter.ProhibitedMoves.EnableCheckmateByHandHu;
            EnableLeaveOte.Value = parameter.ProhibitedMoves.EnableLeaveOte;
        }
    }
}
