using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Prism.Mvvm;
using Prism.Commands;
using Shogi.Business.Application;

namespace MiniShogiApp.Presentation.ViewModel
{
    public class MainWindowViewModel : BindableBase
    {

        public ShogiBoardViewModel Board { get; set; }

        public DelegateCommand TurnBoardCommand{ get; set; }
        private GameService gameService { get; set; } = new GameService();

        public MainWindowViewModel(IMessenger message, Func<GameSet> gameSetGetter)
        {
            Board = new ShogiBoardViewModel(gameService, message, gameSetGetter);
            TurnBoardCommand = new DelegateCommand(() =>
            {
                Board.ForegroundPlayer = Board.ForegroundPlayer == Player.FirstPlayer ? Player.SecondPlayer: Player.FirstPlayer;
            });
        }
    }
}
