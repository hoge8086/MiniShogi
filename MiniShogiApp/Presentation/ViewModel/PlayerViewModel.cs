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
        public ObservableCollection<HandKomaViewModel> Hands { get; set; } = new ObservableCollection<HandKomaViewModel>();

        public DelegateCommand<object> MoveCommand { get; set; }
        public PlayerViewModel(DelegateCommand<object> moveCommand)
        {
            MoveCommand = moveCommand;
        }

    }
}
