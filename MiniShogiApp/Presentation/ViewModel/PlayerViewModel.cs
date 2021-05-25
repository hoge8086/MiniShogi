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

        private bool _isCurrentTurn = false;
        public bool IsCurrentTurn
        {
            get { return _isCurrentTurn; }
            set { SetProperty(ref _isCurrentTurn, value); }
        }

    }
}
