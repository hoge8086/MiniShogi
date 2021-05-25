using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace MiniShogiApp.Presentation.ViewModel
{
    public class PlayerViewModel : BindableBase
    {
        public Player Player { get; set; }
        public ObservableCollection<HandKomaViewModel> Hands { get; set; } = new ObservableCollection<HandKomaViewModel>();

    }
}
