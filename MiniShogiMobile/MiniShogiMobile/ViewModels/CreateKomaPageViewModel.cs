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

        private KomaType komaType;
        public CreateKomaPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            Game = new GameViewModel<CellViewModel, HandsViewModel<HandKomaViewModel>, HandKomaViewModel>();
            Game.Board.UpdateSize(7, 7);
        }

        public override void Prepare(string parameter)
        {
            if (parameter != null)
                komaType = App.CreateGameService.KomaTypeRepository.FindById(parameter);
            else
                komaType = new KomaType();

            Game.Board.Cells[Game.Board.Height / 2][Game.Board.Width / 2].Koma.Value = new KomaViewModel(komaType.Id, PlayerType.Player1, false);
        }

    }
}
