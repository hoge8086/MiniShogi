using System;
using Prism.Mvvm;
using Reactive.Bindings;
using Shogi.Business.Domain.Model.Boards;
using Shogi.Business.Domain.Model.Games;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace MiniShogiMobile.ViewModels
{
    public class CellViewModel : BindableBase
    {
        public BoardPosition Position { get; set; }
        public ReactiveProperty<KomaViewModel> Koma { get; set; }

        public CellViewModel()
        {
            Koma = new ReactiveProperty<KomaViewModel>();
        }
        public virtual void ToEmpty()
        {
            Koma.Value = null;
        }
    }

}
