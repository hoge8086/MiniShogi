using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Prism.Commands;
using Shogi.Bussiness.Domain.Model.Games;
using Shogi.Bussiness.Domain.Model.Boards;
using Shogi.Business.Domain.GameFactory;

namespace MiniShogiApp.Presentation.ViewModel
{
    public class ShogiBoardViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public DelegateCommand MoveCommand { get; set; }
        public ObservableCollection<ObservableCollection<CellViewModel>> Board { get; set; } = new ObservableCollection<ObservableCollection<CellViewModel>>();

        public ShogiBoardViewModel()
        {
            var game = new GameFactory().Create(GameType.AnimalShogi);
            Update(game);
        }

        public void Update(Game game)
        {
            Board.Clear();

            for(int y=0; y<game.Board.Height; y++)
            {
                var row = new ObservableCollection<CellViewModel>();
                for (int x = 0; x<game.Board.Width; x++)
                    row.Add(new CellViewModel() { Koma=null, Position = new BoardPosition(x,y)});
                Board.Add(row);
            }

            foreach(var koma in game.State.KomaList)
            {
                var boardPos = koma.Position as BoardPosition;

                if(boardPos != null)
                {
                    var cell = Board[boardPos.Y][boardPos.X];
                    cell.Koma = new KomaViewModel()
                    {
                        IsTransformed = koma.IsTransformed,
                        Name = koma.KomaType.Id,
                        Player = koma.Player == Shogi.Bussiness.Domain.Model.Players.Player.FirstPlayer ? Player.FirstPlayer : Player.SecondPlayer,
                    };
                }
            }
        }
    }
}
