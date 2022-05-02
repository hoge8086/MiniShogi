using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Navigation;
using Prism.NavigationEx;
using Prism.Services;
using System.Collections.ObjectModel;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Reactive.Linq;
using Shogi.Business.Domain.Model.Komas;
using Shogi.Business.Domain.Model.Moves;
using Shogi.Business.Domain.Model.PlayerTypes;

namespace MiniShogiMobile.ViewModels
{
    public class CreateKomaPageViewModel : NavigationViewModel<string>
    {
        public GameViewModel<CellViewModel, HandsViewModel<HandKomaViewModel>, HandKomaViewModel> Game { get; set; }
        public GameViewModel<CellViewModel, HandsViewModel<HandKomaViewModel>, HandKomaViewModel> PromotedGame { get; set; }

        public ReactiveProperty<KomaViewModel> Koma { get; private set; }
        public ReactiveProperty<KomaViewModel> PromotedKoma { get; private set; }
        public CreateKomaPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            Game = new GameViewModel<CellViewModel, HandsViewModel<HandKomaViewModel>, HandKomaViewModel>();
            PromotedGame = new GameViewModel<CellViewModel, HandsViewModel<HandKomaViewModel>, HandKomaViewModel>();
            Koma = new ReactiveProperty<KomaViewModel>();
            PromotedKoma = new ReactiveProperty<KomaViewModel>();
            int size = 7;
            Game.Board.UpdateSize(size, size);
            PromotedGame.Board.UpdateSize(size, size);
        }

        public override void Prepare(string parameter)
        {
            KomaType komaType;
            if (parameter != null)
                komaType = App.CreateGameService.KomaTypeRepository.FindById(parameter);
            else
                komaType = new KomaType();

            Koma.Value = new KomaViewModel(komaType.Id, PlayerType.Player1, false);
            PromotedKoma.Value = new KomaViewModel(komaType.Id, PlayerType.Player1, true);
            Game.Board.Cells[Game.Board.Height / 2][Game.Board.Width / 2].Koma.Value = Koma.Value;
            PromotedGame.Board.Cells[Game.Board.Height / 2][Game.Board.Width / 2].Koma.Value = PromotedKoma.Value;
        }

    }
}
