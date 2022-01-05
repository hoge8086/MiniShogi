using System;
using Prism.Mvvm;
using Reactive.Bindings;
using Shogi.Business.Domain.Model.Boards;
using Shogi.Business.Domain.Model.Games;
using System.Collections.Generic;
using System.Reactive.Linq;

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

}
