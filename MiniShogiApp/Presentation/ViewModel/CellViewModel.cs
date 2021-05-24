using Shogi.Bussiness.Domain.Model.Boards;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace MiniShogiApp.Presentation.ViewModel
{
    public enum Player
    {
        FirstPlayer,
        SecondPlayer,
    }
    public class KomaViewModel
    {
        public bool IsTransformed { get; set; }
        public Player Player { get; set; }

        public string Name { get; set; }
    }
    public class CellViewModel
    {
        public BoardPosition Position { get; set; }
        public KomaViewModel Koma { get; set; }
        public bool IsTransformed => Koma == null ? false : Koma.IsTransformed;
        public Player Player => Koma == null ? Player.FirstPlayer : Koma.Player;
        public string KomaName => Koma == null ? "" : Koma.Name;
        public bool CanMove { get; private set; } = false;
    }
}
