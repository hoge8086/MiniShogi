using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Prism.Mvvm;
using Prism.Commands;
using Shogi.Business.Application;
using Shogi.Business.Infrastructure;

namespace MiniShogiApp.Presentation.ViewModel
{
    public class MainWindowViewModel : BindableBase
    {
        public ShogiBoardViewModel Board { get; set; }

        public DelegateCommand TurnBoardCommand{ get; set; }

        public MainWindowViewModel(IMessenger message, Func<StartGameWindowViewModel> gameSetGetter)
        {
            Board = new ShogiBoardViewModel(message, gameSetGetter);
            TurnBoardCommand = new DelegateCommand(() =>
            {
                Board.ForegroundPlayer = Board.ForegroundPlayer == Player.FirstPlayer ? Player.SecondPlayer: Player.FirstPlayer;
            });
        }
    }
}
