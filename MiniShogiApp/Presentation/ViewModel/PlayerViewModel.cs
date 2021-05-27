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

        public string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private bool _isCurrentTurn = false;

        public bool IsCurrentTurn
        {
            get { return _isCurrentTurn; }
            set { SetProperty(ref _isCurrentTurn, value); }
        }

    }
}
