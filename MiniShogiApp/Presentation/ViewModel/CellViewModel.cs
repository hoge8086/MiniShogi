using Prism.Mvvm;
using Shogi.Bussiness.Domain.Model.Boards;
using Shogi.Bussiness.Domain.Model.Games;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace MiniShogiApp.Presentation.ViewModel
{
    public interface ISelectable
    {
        bool IsSelected { get; set; }

    }
    public enum Player
    {
        FirstPlayer,
        SecondPlayer,
    }
    public static partial class PlayerExtend
    {
        public static Shogi.Bussiness.Domain.Model.Players.Player ToDomain(this Player player)
        {
            switch (player) {
                case Player.FirstPlayer:
                    return Shogi.Bussiness.Domain.Model.Players.Player.FirstPlayer;
                case Player.SecondPlayer:
                    return Shogi.Bussiness.Domain.Model.Players.Player.SecondPlayer;
            }
            return null;
        }
    }

    public class KomaViewModel
    {
        public bool IsTransformed { get; set; }
        public Player Player { get; set; }

        public string Name { get; set; }
    }
    public class CellViewModel : BindableBase, ISelectable
    {
        public BoardPosition Position { get; set; }
        public KomaViewModel Koma { get; set; }
        public string KomaName => Koma == null ? "" : Koma.Name;


        private List<MoveCommand> moveCommands = null;
        public List<MoveCommand> MoveCommands
        {
            get { return moveCommands; }
            set { moveCommands = value; RaisePropertyChanged(nameof(CanMove)); }
        }
        public bool CanMove => MoveCommands != null;

        private bool _isSelected = false;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty(ref _isSelected, value); }
        }
    }
}
