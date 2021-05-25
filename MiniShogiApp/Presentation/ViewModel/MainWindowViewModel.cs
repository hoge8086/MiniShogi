using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Prism.Mvvm;
using Prism.Commands;

namespace MiniShogiApp.Presentation.ViewModel
{
    public class MainWindowViewModel : BindableBase
    {

        public ShogiBoardViewModel Board { get; set; }

        public DelegateCommand TurnBoardCommand{ get; set; }

        public MainWindowViewModel()
        {
            Board = new ShogiBoardViewModel();
            TurnBoardCommand = new DelegateCommand(() =>
            {
                Board.ForegroundPlayer = Board.ForegroundPlayer == Player.FirstPlayer ? Player.SecondPlayer: Player.FirstPlayer;
            });
        }
    }
}
