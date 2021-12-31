using System;
using Prism.Mvvm;
using Reactive.Bindings;
using Shogi.Business.Domain.Model.Boards;
using Shogi.Business.Domain.Model.Games;
using System.Collections.Generic;

namespace MiniShogiMobile.ViewModels
{
    public class CellBaseViewModel : BindableBase
    {
        public BoardPosition Position { get; set; }
        public ReactiveProperty<KomaViewModel> Koma { get; set; }

        public CellBaseViewModel()
        {
            Koma = new ReactiveProperty<KomaViewModel>();
        }
    }
    public class CellPlayingViewModel : CellBaseViewModel
    {
        public ReactiveProperty<List<MoveCommand>> MoveCommands { get; set; }

        public ReactiveProperty<bool> CanMove { get; set; }
        public CellPlayingViewModel()
        {
            MoveCommands = new ReactiveProperty<List<MoveCommand>>();
            CanMove = new ReactiveProperty<bool>();
            MoveCommands.Subscribe((x) => { CanMove.Value = true; });
        }
    }

}
