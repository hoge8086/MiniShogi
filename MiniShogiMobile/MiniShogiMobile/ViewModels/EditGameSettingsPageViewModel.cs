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
    public class EnumWinConditionTypeProvider : EnumListProvider<WinConditionType> { }
    public class EditGameSettingsPageViewModel : NavigationViewModel<GameTemplate, GameTemplate>
    {
        public AsyncReactiveCommand OkCommand { get; }
        public ReactiveProperty<string> Name { get; }
        public ReactiveProperty<int> Height { get; }
        public ReactiveProperty<int> Width { get; }
        public ReactiveProperty<int> TerritoryBoundary { get; }
        public ReactiveProperty<WinConditionType> WinCondition { get; }
        public ReactiveProperty<bool> EnableNiHu { get; }
        public ReactiveProperty<bool> EnableCheckmateByHandHu { get; }
        public ReactiveProperty<bool> EnableKomaCannotMove { get; }
        public ReactiveProperty<bool> EnableLeaveOte { get; }

        public EditGameSettingsPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            Name = new ReactiveProperty<string>();
            Height = new ReactiveProperty<int>();
            Width = new ReactiveProperty<int>();
            TerritoryBoundary = new ReactiveProperty<int>();
            WinCondition = new ReactiveProperty<WinConditionType>();
            EnableNiHu = new ReactiveProperty<bool>();
            EnableCheckmateByHandHu = new ReactiveProperty<bool>();
            EnableKomaCannotMove = new ReactiveProperty<bool>();
            EnableLeaveOte = new ReactiveProperty<bool>();
            
            OkCommand = new AsyncReactiveCommand();
            OkCommand.Subscribe(async () =>
            {
                var gameTemplate = new GameTemplate();
                gameTemplate.Name = Name.Value;
                gameTemplate.Height = Height.Value;
                gameTemplate.Width = Width.Value;
                gameTemplate.TerritoryBoundary = TerritoryBoundary.Value;
                gameTemplate.WinCondition = WinCondition.Value;
                gameTemplate.ProhibitedMoves.EnableNiHu = EnableNiHu.Value;
                gameTemplate.ProhibitedMoves.EnableKomaCannotMove = EnableKomaCannotMove.Value;
                gameTemplate.ProhibitedMoves.EnableCheckmateByHandHu = EnableCheckmateByHandHu.Value;
                gameTemplate.ProhibitedMoves.EnableLeaveOte = EnableLeaveOte.Value;
                await GoBackAsync(gameTemplate);
            }).AddTo(this.Disposable);
        }

        public override void Prepare(GameTemplate parameter)
        {
            Name.Value = parameter.Name;
            Height.Value = parameter.Height;
            Width.Value = parameter.Width;
            TerritoryBoundary.Value = parameter.TerritoryBoundary;
            WinCondition.Value = parameter.WinCondition;
            EnableNiHu.Value = parameter.ProhibitedMoves.EnableNiHu;
            EnableKomaCannotMove.Value = parameter.ProhibitedMoves.EnableKomaCannotMove;
            EnableCheckmateByHandHu.Value = parameter.ProhibitedMoves.EnableCheckmateByHandHu;
            EnableLeaveOte.Value = parameter.ProhibitedMoves.EnableLeaveOte;
        }
    }
}
